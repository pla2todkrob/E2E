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
    $('#answerUl').empty();
    const listItem = $(button).closest('li');
    const nestedList = listItem.find('.nested-list');
    const siblingItems = listItem.siblings();

    // If other lists are open, close them and revert their button text to normal
    siblingItems.find('.nested-list').empty();
    siblingItems.find('button.btn-expand').find('b').replaceWith(function () {
        return $('<span>', {
            html: this.innerHTML
        });
    });

    // Toggle current list
    if (nestedList.children().length === 0) {
        getNextLevel(id, nestedList);

        // Change the text of the clicked button to bold
        $(button).find('span').replaceWith(function () {
            return $('<b>', {
                html: this.innerHTML
            });
        });
    } else {
        nestedList.empty();

        // Revert the text of the clicked button to normal
        $(button).find('b').replaceWith(function () {
            return $('<span>', {
                html: this.innerHTML
            });
        });
    }
}

function loadNextLevel(questions, container, answers) {
    if (questions.length === 0) {
        //console.log('questions.length === 0');
        displayAnswers(answers);
        return;
    }

    $.each(questions, function (i, question) {
        //console.log(i, question);
        const listItem = $('<li class="list-group-item list-group-item-action">');
        const button = $('<button class="btn btn-block btn-expand text-left" onclick="expandQuestion(\'' + question.ChatBotQuestion_Id + '\', this)">');
        const questionSpan = $('<span>').text(question.Question);
        const nestedList = $('<ul class="list-group nested-list">');

        button.append(questionSpan);
        listItem.append(button, nestedList);
        container.append(listItem);
    });

    displayAnswers(answers);
    //console.log('displayAnswers', answers);
}
