$(document).ready(function () {
  $("#sidebar").mCustomScrollbar({
    theme: "minimal"
  });

  $('.sidebarCollapse').on('click', function (e) {

    $('#sidebar, #content').toggleClass('active');

    //$('.collapse.in').toggleClass('in');
    //$('a[aria-expanded=true]').attr('aria-expanded', 'false');
  });
});
