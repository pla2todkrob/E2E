$(function () {
    AdjustMenu();
    $(window).resize(function () {
        AdjustMenu();
    });
});

function AdjustMenu() {
    var sidebarHeight = $('#sidebar').innerHeight();
    var brandHeight = $('#brand').innerHeight();
    var menuHeight = sidebarHeight - brandHeight;
    $('#menu').innerHeight(menuHeight);
}