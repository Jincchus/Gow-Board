document.addEventListener('DOMContentLoaded', function () {
    // 본문 삭제 버튼
    const contentDeleteLink = document.querySelector('#content-delete-link');
    if (contentDeleteLink) {
        contentDeleteLink.addEventListener('click', function (e) {
            e.preventDefault();
            if (confirm("정말로 삭제하시겠습니까?")) {
                window.location.href = this.getAttribute('href');
            }
        });
    }

    // 파일 다운로드
    const fileLinks = document.querySelectorAll('a[data-file-type="file"]');
    fileLinks.forEach(function (link) {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            const fileName = this.getAttribute('data-file-name');
            const downloadUrl = '/Board/DownloadFile?fileName=' + encodeURIComponent(fileName);
            window.location.href = downloadUrl;
        });
    });

    // 댓글 등록 버튼 이벤트 리스너
    const commentAjaxBtn = document.querySelector("#comment-ajax-btn");
    if (commentAjaxBtn) {
        commentAjaxBtn.addEventListener("click", createComment);
    }

    // 대댓글 토글 버튼
    document.addEventListener('click', function (event) {
        if (event.target && event.target.classList.contains('bi-pencil-fill')) {
            toggleReplyForm(event);
        }
    });

    // 대댓글 등록 버튼
    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('reply-comment-btn')) {
            createReplyComment(e);
        }
    });
});

function createComment(event) {
    if (!confirm("댓글을 등록하시겠습니까?")) {
        return;
    }

    const commentContent = document.querySelector("#ParentComment").value;

    const commentData = {
        BoardContentId: boardContentId,
        Content: commentContent,
        ParentCommentId: null,
    };

    fetch('/Comment/CreateComment', {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(commentData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert(data.message);
                location.reload(); // TODO: 댓글 추가 후 페이지 새로고침(새로고침 말고 동적으로 리플 확인 할 수 있게 변경)
            } else if (!data.isAuthenticated) {
                alert(data.message);
                window.location.href = '/Member/Login';
            } else {
                alert(data.message);
            }
        }).catch(error => {
            alert('catch error: ' + error.message);
        });
}

function createReplyComment(event) {
    if (!confirm("댓글을 등록하시겠습니까?")) {
        return;
    }

    const parentCommentId = event.target.getAttribute('data-parent-comment-id');
    const replyInput = document.querySelector(`#reply-input-${parentCommentId}`);
    const replyContent = replyInput.value;

    const commentData = {
        BoardContentId: boardContentId,
        Content: replyContent,
        ParentCommentId: parentCommentId,
    };

    fetch('/Comment/CreateComment', {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(commentData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert(data.message);
                location.reload(); // TODO: 댓글 추가 후 페이지 새로고침(새로고침 말고 동적으로 리플 확인 할 수 있게 변경)
            } else if (!data.isAuthenticated) {
                alert(data.message);
                window.location.href = '/Member/Login';
            } else {
                alert(data.message);
            }
        }).catch(error => {
            alert('catch error: ' + error.message);
        });
}

// 대댓글 작성란 토글 함수
function toggleReplyForm(e) {
    e.preventDefault();
    const commentId = e.target.getAttribute('data-comment-id');
    const replyFormContainer = document.querySelector(`#reply-form-container-${commentId}`);
    if (replyFormContainer) {
        replyFormContainer.style.display = replyFormContainer.style.display === 'none' ? 'block' : 'none';
    }
}