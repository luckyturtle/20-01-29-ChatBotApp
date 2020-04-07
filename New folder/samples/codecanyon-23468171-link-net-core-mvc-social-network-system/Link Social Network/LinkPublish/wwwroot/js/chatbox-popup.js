
///http://qnimate.com/facebook-style-chat-box-popup-using-javascript-and-css/

//this function can remove a array element.
Array.remove = function (array, from, to) {
    var rest = array.slice((to || from) + 1 || array.length);
    array.length = from < 0 ? array.length + from : from;
    return array.push.apply(array, rest);
};

//this variable represents the total number of popups can be displayed according to the viewport width
var total_popups = 0;

//arrays of popups ids
var popups = [];


//this is used to close a popup
function close_popup(id) {
    for (var iii = 0; iii < popups.length; iii++) {
        if (id == popups[iii]) {
            Array.remove(popups, iii);

            document.getElementById(id).style.display = "none";
            window["popup-comment-" + id] = null;
            calculate_popups();

            return;
        }
    }
}

//displays the popups. Displays based on the maximum number of popups that can be displayed on the current viewport width
function display_popups() {
    var right = 220;

    if ($(window).width() < 600) {
        right = 30;
    }

    var iii = 0;
    for (iii; iii < total_popups; iii++) {
        if (popups[iii] != undefined) {
            var element = document.getElementById(popups[iii]);
            element.style.right = right + "px";
            right = right + 320;
            element.style.display = "block";
        }
    }

    for (var jjj = iii; jjj < popups.length; jjj++) {
        var element = document.getElementById(popups[jjj]);
        element.style.display = "none";
    }
}

//creates markup for a new popup. Adds the id to popups array.
function register_popup(id, name, root, currentUserId, typeId) {

    for (var iii = 0; iii < popups.length; iii++) {
        //already registered. Bring it to front.
        if (id == popups[iii]) {
            Array.remove(popups, iii);

            popups.unshift(id);

            calculate_popups();

            return;
        }
    }



    var element = '<div class="popup-box chat-popup" id="' + id + '">';
    element = element + '<div class="popup-head">';
    element = element + '<div class="popup-head-left">' + name + '</div>';
    element = element + '<div class="popup-head-right"><a href="javascript:close_popup(\'' + id + '\');">&#10005;</a></div>';
    element = element + '<div style="clear: both"></div></div><div class=" scrollbar popup-messages" id="' + "popup-comment-" + id + '"></div><br></div>';

    document.getElementsByTagName("footer")[0].insertAdjacentHTML('beforeend', element);

    popups.unshift(id);

    calculate_popups();

    //set the current group message session as seen by the current user 
    $.ajax({
        type: 'put',
        url: root + '/api/Notifications/setNotificationAsRead/' + id + '/1',
        success: function () {
            //refresh the notification-num 
            $.ajax({
                type: 'get',
                url: root + '/api/Notifications/notificationsNo/1',
                success: function (num) {
                    $(".chat-notification-num").text(num);
                }
            });
        }
    });

    window["popup-comment-" + id] = $("#popup-comment-" + id).chatComments({
        textareaPlaceholderText: 'Leave a message',
        creator: currentUserId,
        rootUrl: root,
        //commentContainer: ".group-comments-container",
        filePath: root + '/Comments/DownloadAttachment/',
        commentProfilePicturePath: root + '/Profile/GetUserProfile/',
        fieldMappings: {
            id: 'id',
            parent: 'parent',
            created: 'created',
            modified: 'modified',
            content: 'content',
            file: 'file',
            fileURL: 'fileURL',
            fileMimeType: 'fileMimeType',
            pings: 'pings',
            creator: 'creator',
            fullname: 'fullname',
            profileURL: 'profileURL',
            profilePictureURL: 'profilePictureURL',
            isNew: 'isNew',
            createdByAdmin: 'createdByAdmin',
            createdByCurrentUser: 'createdByCurrentUser',
            upvoteCount: 'upvoteCount',
            userHasUpvoted: 'userHasUpvoted'
        }, refresh: function () {
            $("#popup-comment-" + id).addClass('rendered');
        }, getUsers: function (success, error) {
            $.ajax({
                type: 'get',
                url: root + '/api/users/',
                success: function (userArray) {
                    success(userArray)
                },
                error: error
            });
        },
        hashtagClicked: function (hashtag) {

            var win = window.open(root + '/Comments/Hashtag/' + hashtag, '_blank');
            win.focus();

        }, loadMoreComments: function (comment) {
            $.ajax({
                type: 'get',
                url: root + '/api/comments/' + id + '/' + typeId + '/' + comment.options.commentCount,
                success: function (commentsArray) {
                    if (commentsArray.length >= 5) {
                        $.each(commentsArray, function (index, value) {
                            comment.prependNewComment(value)
                        });
                    }
                },
                async: false
            });

        },
        getComments: function (success, error) {
            $.ajax({
                type: 'get',
                url: root + '/api/comments/' + id + '/' + typeId + '/0',
                success: function (commentsArray) {
                    success(commentsArray)

                    $("#popup-comment-" + id).scrollTop($("#popup-comment-" + id)[0].scrollHeight);

                },
                error: error,
                async: false
            });

        }, postComment: function (commentJSON, success, error) {

            commentJSON.groupId = id;
            commentJSON.commentGroupTypeId = typeId;
            $.ajax({
                type: 'post',
                url: root + '/api/comments/',
                data: commentJSON,
                success: function (comment) {
                    success(comment)
                    $("#popup-comment-" + id).scrollTop($("#popup-comment-" + id)[0].scrollHeight);
                },
                error: error
            });


        }, putComment: function (commentJSON, success, error) {
            commentJSON.groupId = id;
            commentJSON.commentGroupTypeId = typeId;
            $.ajax({
                type: 'put',
                url: root + '/api/comments/' + commentJSON.id,
                data: commentJSON,
                success: function (comment) {
                    success(comment)
                },
                error: error
            });

        }, deleteComment: function (commentJSON, success, error) {
            $.ajax({
                type: 'delete',
                url: root + '/api/comments/' + commentJSON.id,
                success: success,
                error: error
            });
        }, upvoteComment: function (commentJSON, success, error) {
            var commentURL = root + '/api/comments/' + commentJSON.id;
            var upvotesURL = commentURL + '/upvotes/';
            var value = (commentJSON.userHasUpvoted == true) ? 1 : 0;

            $.ajax({
                type: 'put',
                url: upvotesURL + value,
                success: function () {
                    success(commentJSON)
                },
                error: error
            });
        },
        downloadAttachment: function (comment) {
            var win = window.open(root + '/Comments/DownloadAttachment/' + comment.id, '_blank');
            win.focus();
        },
        uploadAttachments: function (commentArray, success, error) {
            $(commentArray).each(function (index, commentJson) {

                // Create form data
                var formData = new FormData();
                $(Object.keys(commentJson)).each(function (index, key) {
                    var value = commentJson[key];
                    if (value) formData.append(key, value);
                });

                var tempCommentJson = commentJson;
                tempCommentJson.file = null;

                $.ajax({
                    rootUrl: root,
                    url: root + '/api/comments/Upload/',
                    type: 'post',
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (name) {
                        tempCommentJson.fileURL = name;
                        tempCommentJson.groupId = id;
                        tempCommentJson.commentGroupTypeId = typeId;
                        $.ajax({
                            url: root + '/api/comments/',
                            type: 'post',
                            data: tempCommentJson,
                            success: function (comment) {
                                success(comment)
                                $("#popup-comment-" + id).scrollTop($("#popup-comment-" + id)[0].scrollHeight);
                            },
                            error: error
                        });
                    },
                    error: function (data) {

                    }
                });
            });

        },
    });

}


//calculate the total number of popups suitable and then populate the toatal_popups variable.
function calculate_popups() {
    var width = window.innerWidth;
    if (width < 540) {
        total_popups = 1;
    }
    else {
        width = width - 200;
        //320 is width of a single popup box
        total_popups = parseInt(width / 320);
    }

    display_popups();

}

//recalculate when window is loaded and also when window is resized.
window.addEventListener("resize", calculate_popups);
window.addEventListener("load", calculate_popups);

