function performSearch() {
    var searchText = $('#searchInput').val().toLowerCase();

    $('#answerUl > li').each(function () {
        var currentLiText = $(this).text(),
            showCurrentLi = currentLiText.toLowerCase().indexOf(searchText) !== -1;

        $(this).toggle(showCurrentLi);
    });
}

$(document).on('keypress', '#searchInput', function (event) {
    if (event.which == 13) {
        event.preventDefault();
        performSearch();
    }
});

$(document).on('click', '#searchButton', function () {
    performSearch();
});

$('#closeArea').click(() => {
    $('#qaList').removeClass('active');
});

function expandQuestion(id, button) {
    const listItem = $(button).closest('li');
    const nestedList = listItem.find('.nested-list');

    if (nestedList.children().length === 0) {
        getNextLevel(id, nestedList);
    } else {
        $('#answerUl').empty();
        nestedList.empty();
    }
}

function loadNextLevel(questions, container, answers) {
    if (questions.length === 0) {
        displayAnswers(answers);
        return;
    }

    $.each(questions, function (i, question) {
        const listItem = $('<li class="list-group-item list-group-item-action">');
        const button = $('<button class="btn btn-block btn-expand text-left" onclick="expandQuestion(\'' + question.ChatBotQuestion_Id + '\', this)">');
        const questionSpan = $('<span>').text(question.Question);
        const nestedList = $('<ul class="list-group nested-list">');

        button.append(questionSpan);
        listItem.append(button, nestedList);
        container.append(listItem);
    });

    displayAnswers(answers);
}
