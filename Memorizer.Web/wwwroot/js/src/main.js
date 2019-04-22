$(document).ready(function () {

    window.history.replaceState(null, null, window.location.href);

    if (sessionStorage.hasOwnProperty("alert")) {
        showMessage(sessionStorage.getItem("alert"));
        sessionStorage.removeItem("alert");
    }
});

function initiate(baseUrl, memoModel) {

    $("#showAnswer").focus();

    $("#showAnswer").click(function () {
        $("#answerArea").val(memoModel.answer);
        $("#answerForm").css("visibility", "visible");
        $("input[value=Cool]").focus();
    });

    $("#questionArea,#answerArea").focus(function () {
        $(".update-group").css("visibility", "visible");
    });

    $("#questionArea,#answerArea").blur(function () {
        if ($(".update-group:hover").length === 0) {
            $(".update-group").css("visibility", "hidden");
        }
    });

    $("#replaceMemos").click(function () {
        var question = $("#questionArea").val();
        var answer = $("#answerArea").val();
        $("#questionArea").val(answer);
        $("#answerArea").val(question);
    });

    $("#switchModeButton").click(function (event) {
        window.location = baseUrl.concat(
            $(this).text().includes("Learn") ? "Learn" : "Repeat");

        event.preventDefault();
    });

    $("#updateButton").click(function (event) {
        memoModel.question = $("#questionArea").val();
        memoModel.answer = $("#answerArea").val();
        asyncPost(baseUrl + "Update", memoModel);

        $(".update-group").css("visibility", "hidden");
        event.preventDefault();
    });

    $("#deleteButton").click(function (event) {
        asyncPost(baseUrl + "Delete/" + memoModel.id,
            null,
            function (response) {
                sessionStorage.setItem("alert", response);
                location.reload(true);
            });

        event.preventDefault();
    });

    $("#answerForm").submit(function () {
        $(".lds-ellipsis").css("visibility", "visible");
    });

    initiateModals(baseUrl);
};

function showMessage(response) {
    $("#message").text(response);
    var className = !response.startsWith("Error") ? "alert-success" : "alert-danger";
    $(".alert").addClass(className);

    $(".alert").removeClass("d-none");
}

function asyncPost(url, data, callback) {
    $(".lds-ellipsis").css("visibility", "visible");

    return $.ajax({
        url: url,
        type: "POST",
        data: data,
        complete: function () {
            $(".lds-ellipsis").css("visibility", "hidden");
        },
        success: function (response) {

            if (callback != undefined)
                callback(response);

            showMessage(response);
        }
    });
}

function asyncGet(actionLink, callback) {
    return $.ajax({
        url: actionLink,
        type: "GET",
        success: function (data) {
            if (callback != undefined)
                callback(data);
        }
    });
}

function finish(mode) {
    $(".summary").css("visibility", "hidden");
    $("#finishMessage").text(mode);
    $("#finishModal").modal();
}
