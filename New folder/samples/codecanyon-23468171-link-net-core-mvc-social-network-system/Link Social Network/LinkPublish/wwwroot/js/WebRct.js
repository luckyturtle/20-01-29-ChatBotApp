/* From https://code.google.com/p/webrtc-samples */

var RTCPeerConnection = null;
var getUserMedia = null;
var attachMediaStream = null;
var reattachMediaStream = null;
var webrtcDetectedBrowser = null;

if (navigator.mozGetUserMedia) {
    console.log("This appears to be Firefox");

    webrtcDetectedBrowser = "firefox";

    // The RTCPeerConnection object.
    RTCPeerConnection = mozRTCPeerConnection;

    // The RTCSessionDescription object.
    RTCSessionDescription = mozRTCSessionDescription;

    // The RTCIceCandidate object.
    RTCIceCandidate = mozRTCIceCandidate;

    // Get UserMedia (only difference is the prefix).
    // Code from Adam Barth.
    getUserMedia = navigator.mozGetUserMedia.bind(navigator);

    // Attach a media stream to an element.
    attachMediaStream = function (element, stream) {
        console.log("Attaching media stream");
        element.mozSrcObject = stream;
        element.play();
    };

    reattachMediaStream = function (to, from) {
        console.log("Reattaching media stream");
        to.mozSrcObject = from.mozSrcObject;
        to.play();
    };

    // Fake get{Video,Audio}Tracks
    MediaStream.prototype.getVideoTracks = function () {
        return [];
    };

    MediaStream.prototype.getAudioTracks = function () {
        return [];
    };
} else if (navigator.webkitGetUserMedia) {
    console.log("This appears to be Chrome");

    webrtcDetectedBrowser = "chrome";

    // The RTCPeerConnection object.
    RTCPeerConnection = webkitRTCPeerConnection;

    // Get UserMedia (only difference is the prefix).
    // Code from Adam Barth.
    getUserMedia = navigator.webkitGetUserMedia.bind(navigator);

    // Attach a media stream to an element.
    attachMediaStream = function (element, stream) {
        try {
            element.srcObject = stream;
        } catch (error) {
            element.src = webkitURL.createObjectURL(stream);
        }
    };

    reattachMediaStream = function (to, from) {
        to.src = from.src;
    };

    // The representation of tracks in a stream is changed in M26.
    // Unify them for earlier Chrome versions in the coexisting period.
    if (!webkitMediaStream.prototype.getVideoTracks) {
        webkitMediaStream.prototype.getVideoTracks = function () {
            return this.videoTracks;
        };
        webkitMediaStream.prototype.getAudioTracks = function () {
            return this.audioTracks;
        };
    }

    // New syntax of getXXXStreams method in M26.
    if (!webkitRTCPeerConnection.prototype.getLocalStreams) {
        webkitRTCPeerConnection.prototype.getLocalStreams = function () {
            return this.localStreams;
        };
        webkitRTCPeerConnection.prototype.getRemoteStreams = function () {
            return this.remoteStreams;
        };
    }
} else {
    console.log("Browser does not appear to be WebRTC-capable");
}
//https://github.com/mgiuliani/webrtc-video-chat/blob/master/VideoChat/Scripts/webrtcdemo/connectionManager.js
var WebRtc = WebRtc || {};


WebRtc.ConnectionManager = (function () {
    var _signaler,
        _connections = {},
       
        _iceServers = [{ url: 'stun:74.125.142.127:19302' }], // stun.l.google.com - Firefox does not support DNS names.

        /* Callbacks */
        _onReadyForStreamCallback = function () { console.log('UNIMPLEMENTED: _onReadyForStreamCallback'); },
        _onStreamAddedCallback = function () { console.log('UNIMPLEMENTED: _onStreamAddedCallback'); },
        _onStreamRemovedCallback = function () { console.log('UNIMPLEMENTED: _onStreamRemovedCallback'); },

        // Initialize the ConnectionManager with a signaler and callbacks to handle events
        _initialize = function (signaler, onReadyForStream, onStreamAdded, onStreamRemoved) {
            _signaler = signaler;
           
            _onReadyForStreamCallback = onReadyForStream || _onReadyForStreamCallback;
            _onStreamAddedCallback = onStreamAdded || _onStreamAddedCallback;
            _onStreamRemovedCallback = onStreamRemoved || _onStreamRemovedCallback;
        },

        // Create a new WebRTC Peer Connection with the given partner
        _createConnection = function (partnerClientId) {
            console.log('WebRTC: creating connection...');

            // Create a new PeerConnection
            var connection = new RTCPeerConnection({ iceServers: _iceServers });

            // ICE Candidate Callback
            connection.onicecandidate = function (event) {
                if (event.candidate) {
                    // Found a new candidate
                    console.log('WebRTC: new ICE candidate');
                    _signaler.invoke("sendSignal", JSON.stringify({ "candidate": event.candidate }), partnerClientId).catch(err => console.error(err));


                } else {
                    // Null candidate means we are done collecting candidates.
                    console.log('WebRTC: ICE candidate gathering complete');
                }
            };

            // State changing
            connection.onstatechange = function () {
                // Not doing anything here, but interesting to see the state transitions
                var states = {
                    'iceConnectionState': connection.iceConnectionState,
                    'iceGatheringState': connection.iceGatheringState,
                    'readyState': connection.readyState,
                    'signalingState': connection.signalingState
                };

                console.log(JSON.stringify(states));
            };

            // Stream handlers
            connection.onaddstream = function (event) {
                console.log('WebRTC: adding stream');
                // A stream was added, so surface it up to our UI via callback
                _onStreamAddedCallback(connection, event);
            };

            connection.onremovestream = function (event) {
                console.log('WebRTC: removing stream');
                // A stream was removed
                _onStreamRemovedCallback(connection, event.stream.id);
            };

            // Store away the connection
            _connections[partnerClientId] = connection;

            // And return it
            return connection;
        },

        // Process a newly received SDP signal
        _receivedSdpSignal = function (connection, partnerClientId, sdp) {
            console.log('WebRTC: processing sdp signal');
            connection.setRemoteDescription(new RTCSessionDescription(sdp), function () {
                if (connection.remoteDescription.type == "offer") {
                    console.log('WebRTC: received offer, sending response...');
                    _onReadyForStreamCallback(connection);
                    connection.createAnswer(function (desc) {
                        connection.setLocalDescription(desc, function () {
                            _signaler.invoke("sendSignal", JSON.stringify({ "sdp": connection.localDescription }), partnerClientId).catch(err => console.error(err));

                        });
                    },
                        function (error) { console.log('Error creating session description: ' + error); });
                } else if (connection.remoteDescription.type == "answer") {
                    console.log('WebRTC: received answer');
                }
            });
        },

        // Hand off a new signal from the signaler to the connection
        _newSignal = function (partnerClientId, data) {
            var signal = JSON.parse(data),
                connection = _getConnection(partnerClientId);

            console.log('WebRTC: received signal');

            // Route signal based on type
            if (signal.sdp) {
                _receivedSdpSignal(connection, partnerClientId, signal.sdp);
            } else if (signal.candidate) {
                _receivedCandidateSignal(connection, partnerClientId, signal.candidate);
            }
        },

        // Process a newly received Candidate signal
        _receivedCandidateSignal = function (connection, partnerClientId, candidate) {
            console.log('WebRTC: processing candidate signal');
            connection.addIceCandidate(new RTCIceCandidate(candidate));
        },

        // Retreive an existing or new connection for a given partner
        _getConnection = function (partnerClientId) {
            var connection = _connections[partnerClientId] || _createConnection(partnerClientId);
            return connection;
        },

        // Close all of our connections
        _closeAllConnections = function () {
            for (var connectionId in _connections) {
                _closeConnection(connectionId);
            }
        },

        // Close the connection between myself and the given partner
        _closeConnection = function (partnerClientId) {
            var connection = _connections[partnerClientId];

            if (connection) {
                // Let the user know which streams are leaving
                // todo: foreach connection.remoteStreams -> onStreamRemoved(stream.id)
                _onStreamRemovedCallback(null, null);

                // Close the connection
                connection.close();
                delete _connections[partnerClientId]; // Remove the property
            }
        },

        // Send an offer for audio/video
        _initiateOffer = function (partnerClientId, stream) {
            // Get a connection for the given partner
            var connection = _getConnection(partnerClientId);

            // Add our audio/video stream
            connection.addStream(stream);

            console.log('stream added on my end');

            // Send an offer for a connection
            connection.createOffer(function (desc) {
                connection.setLocalDescription(desc, function () {
                    _signaler.invoke("sendSignal", JSON.stringify({ "sdp": connection.localDescription }), partnerClientId).catch(err => console.error(err));

                });
            }, function (error) { console.log('Error creating session description: ' + error); });
        };

    // Return our exposed API
    return {
        initialize: _initialize,
        newSignal: _newSignal,
        closeConnection: _closeConnection,
        closeAllConnections: _closeAllConnections,
        initiateOffer: _initiateOffer
    };
})();
//todo: improve the code, and make it clean.

//Reference:
//https://github.com/mgiuliani/webrtc-video-chat
//https://www.tutorialspoint.com/webrtc/

var WebRctApp = WebRctApp || {};
var audio_watting = new Audio('../sound/watting.wav');
var audio_ringing = new Audio('../sound/Ringing.wav');

WebRctApp.startConnection = (function () {

    var connectionManager = WebRtc.ConnectionManager
        , _mediaStream
        , _hub
    setup = function () {
        // Set Up SignalR Signaler
        var hub = new signalR.HubConnectionBuilder().withUrl(root + '/WebRtcHub').build();
        _hub = hub;
        hub.start().then(function () {
            // Tell the hub what our username is
            hub.invoke("join", $('#callerName').val()).catch(err => console.error(err));

        }).catch(function (event) {

        });

        // Setup client SignalR operations
        setupHubCallbacks(hub);

        $(".callUser").click(function () {
            startWattingSound();

            hub.invoke("callUser", $('#calleeUserName').val()).catch(err => console.log(err));
        });

        $(".hangUp").click(function () {

            alertify.confirm('Confirm Message', "Are you sure, you want to end the call?",
                function () {
                    stopWattingSound();

                    hub.invoke("HangUp", $('#calleeUserName').val()).catch(err => console.error(err));

                    $('#ptpCall').modal('hide');

                    //$(".localVideo").src = null;

                    connectionManager.closeAllConnections();

                    _mediaStream.getTracks().forEach(function (track) {
                        track.stop();
                    });

                }, function () {//do nothing 
                });
        });

    },
        startRingingSound = function () {
            audio_ringing.loop = true;
            audio_ringing.play();
        },
        stopRingingSound = function () {
            audio_ringing.pause();
            audio_ringing.currentTime = 0;
        },
        startWattingSound = function () {
        audio_watting.loop = true;
        audio_watting.play();
        },
        stopWattingSound = function () {
        audio_watting.pause();
        audio_watting.currentTime = 0;
        },
        notifyPopup = function (title, msg, callingUser) {
            try {
                var icon = "";
                if (callingUser != null) {
                    icon = root + '/Profile/GetUserProfile/' + callingUser.id + ' ';
                } else {
                    icon = root + '/images/default-image.svg ';
                }

                if (!Notification) {
                    alertify.error('Desktop notifications not available in your browser. Try other browser.');
                    return;
                }

                if (Notification.permission !== "granted")
                    Notification.requestPermission();
                else {
                    var notification = new Notification(title, {
                        icon: icon,
                        body: msg,
                    });

                    notification.onclick = function () {
                        window.open(root + '/Profile/' + callingUser.username);
                    };
                }
            } catch (e) { }
        },
        setupHubCallbacks = function (hub) {

            hub.on("calleeNotConnected", function () {
                stopWattingSound();

                alertify.alert('The User you trying to call is not connected', 'Sorry :(  "' + $('#calleeUserName').val() + '" is not connected for now, please call back later')
            });

            hub.on("callerCanceled", function (callingUserName) {

                // Let the user know why the server says the call is over
                alertify.error(callingUserName + " has hung up.");
                // Close the WebRTC connection
                alertify.closeAll();
                stopRingingSound();

            });

            // Hub Callback: WebRTC Signal Received
            hub.on("receiveSignal", function (callingUser, data) {
                connectionManager.newSignal(callingUser.connectionId, data);
            });

            // Hub Callback: Incoming Call
            hub.on("incomingCall", function (callingUser) {
                startRingingSound();
                alertify.notify('incoming call from: ' + callingUser.username, 'custom', 3, '');
                notifyPopup('Incoming call', callingUser.username + ' is calling. Do you want to chat?', callingUser)

                alertify.confirm('Incoming call', callingUser.username + ' is calling. Do you want to chat?',
                    function () {
                        stopRingingSound();

                        // I want to chat
                        // Ask the user for permissions to access the webcam and mic
                        getUserMedia(
                            {
                                // Permissions to request
                                video: true,
                                audio: true
                            },
                            function (stream) { // succcess callback gives us a media stream
                                var callbacks = {
                                    onReadyForStream: function (connection) {
                                        // The connection manager needs our stream
                                        // todo: not sure I like this
                                        connection.addStream(_mediaStream);
                                    },
                                    onStreamAdded: function (connection, event) {

                                        // Bind the remote stream to the partner window
                                        var otherVideo = document.querySelector('.video.partner');
                                        attachMediaStream(otherVideo, event.stream); // from adapter.js
                                    },
                                    onStreamRemoved: function (connection, streamId) {
                                        // todo: proper stream removal.  right now we are only set up for one-on-one which is why this works.

                                        // Clear out the partner window
                                        var otherVideo = document.querySelector('.video.partner');
                                        otherVideo.src = '';
                                    }
                                };

                                // Initialize our client signal manager, giving it a signaler (the SignalR hub) and some callbacks
                                connectionManager.initialize(_hub, callbacks.onReadyForStream, callbacks.onStreamAdded, callbacks.onStreamRemoved);

                                // Store off the stream reference so we can share it later
                                _mediaStream = stream;

                                // Load the stream into a video element so it starts playing in the UI
                                var videoElement = document.querySelector('.video.mine');
                                attachMediaStream(videoElement, _mediaStream);

                                _hub.invoke("answerCall", true, callingUser.connectionId).catch(err => console.error(err));

                                $('#calleeUserName').val(callingUser.username)

                                $('#ptpCall').modal('show');
                            },
                            function (error) { // error callback
                                alertify.alert('<h4>Failed to get hardware access!</h4> Do you have another browser type open and using your cam/mic?<br/><br/>You were not connected to the server, because I didn\'t code to make browsers without media access work well. <br/><br/>Actual Error: ' + JSON.stringify(error));
                                //hid loading..
                            }
                        );
                    }
                    , function () {
                        stopRingingSound();
                        // Go away, I don't want to chat with you
                        hub.invoke("answerCall", false, callingUser.connectionId).catch(err => console.error(err));
                    });

            });

            // Hub Callback: Call Accepted
            hub.on("callAccepted", function (acceptingUser) {
                stopWattingSound();

                alertify.notify('call accepted from: ' + acceptingUser.username, 'custom', 3, '');

                //// Callee accepted our call, let's send them an offer with our video stream
                connectionManager.initiateOffer(acceptingUser.connectionId, _mediaStream);
            });

            hub.on("callDeclined", function (reason) {
                stopWattingSound();

                // Let the user know that the callee declined to talk
                alertify.error(reason);

            });

            // Hub Callback: Call Ended
            hub.on("callEnded", function (connectionId, reason) {
                stopWattingSound();

                // Let the user know why the server says the call is over
                alertify.error(reason);

                // Close the WebRTC connection
                connectionManager.closeConnection(connectionId);
            });


            // Hub Callback: WebRTC Signal Received
            hub.on("receiveSignal", function (callingUser, data) {
                connectionManager.newSignal(callingUser.connectionId, data);
            });


        }

        , startSession = function startSession() {


            // Ask the user for permissions to access the webcam and mic
            getUserMedia(
                {
                    // Permissions to request
                    video: true,
                    audio: true
                },
                function (stream) { // succcess callback gives us a media stream
                    var callbacks = {
                        onReadyForStream: function (connection) {
                            // The connection manager needs our stream
                            connection.addStream(_mediaStream);
                        },
                        onStreamAdded: function (connection, event) {

                            // Bind the remote stream to the partner window
                            var otherVideo = document.querySelector('.video.partner');
                            attachMediaStream(otherVideo, event.stream); // from adapter.js
                        },
                        onStreamRemoved: function (connection, streamId) {
                            // todo: proper stream removal.  right now we are only set up for one-on-one which is why this works.

                            // Clear out the partner window
                            var otherVideo = document.querySelector('.video.partner');
                            otherVideo.src = '';
                        }
                    };

                    // Initialize our client signal manager, giving it a signaler (the SignalR hub) and some callbacks
                    connectionManager.initialize(_hub, callbacks.onReadyForStream, callbacks.onStreamAdded, callbacks.onStreamRemoved);

                    // Store off the stream reference so we can share it later
                    _mediaStream = stream;

                    // Load the stream into a video element so it starts playing in the UI
                    var videoElement = document.querySelector('.video.mine');
                    attachMediaStream(videoElement, _mediaStream);

                },
                function (error) { // error callback
                    alertify.alert('<h4>Failed to get hardware access!</h4> Do you have another browser type open and using your cam/mic?<br/><br/>You were not connected to the server, because I didn\'t code to make browsers without media access work well. <br/><br/>Actual Error: ' + JSON.stringify(error));
                }
            );


        }
        , start = function () {
            // Show warning if WebRTC support is not detected
            if (webrtcDetectedBrowser == undefined || webrtcDetectedBrowser == null) {
                alertify.error('Your browser doesnt appear to support WebRTC');
            }

            // Then proceed to the next step, gathering username
            startSession();
        }
    // Return our exposed API
    return {
        start: start,
        setup: setup,

    };
})();