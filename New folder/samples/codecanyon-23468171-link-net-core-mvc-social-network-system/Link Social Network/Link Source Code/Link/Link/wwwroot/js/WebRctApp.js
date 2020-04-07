/***********************************************************************************************************************
    * Link  is a Social Network for enable distributed user to involve, collaborate and communicate.
    * Copyright(C) 2019 Indie Sudio.All rights reserved.
    * https://indiestd.com/
    * info@indiestd.com
 *********************************************************************************************************************/

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

