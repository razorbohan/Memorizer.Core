function initiateModals(baseUrl) {
    $(window).click(function (event) {
        if ($(".navbar-collapse").hasClass("show") && !$(event.target).hasClass("navbar-toggler")) {
            $("button.navbar-toggler").click();
        }
    });

    $("#addModal").on("hidden.bs.modal", function () {
        $(this).find(".modal-body > textarea").val("");
    });

    $("#findNewMemo").click(function () {
        var key = $("#findKey").val();
        var value = $("#findValue").val();
        initiateJsGrid(key, value, baseUrl);
        $("#findModal").find(".modal-dialog").css("max-width", "90%");
    });

    $("#submitNewMemo").click(function () {
        asyncPost(baseUrl + "Add",
            "question=" + $("#addQuestionArea").val() +
            "&answer=" + $("#addAnswerArea").val());
    });

    $("#findModal").on("hidden.bs.modal", function () {
        $("#findKey").val("");
        if ($("#jsGrid").children().length > 0)
            $("#jsGrid").jsGrid("destroy");
        $(this).find(".modal-dialog").css("max-width", "");
    });
};