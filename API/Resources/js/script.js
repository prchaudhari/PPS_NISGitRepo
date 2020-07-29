$(document).ready(function() {
	$('.nav-link').click(function (e) { 
	  $('.tabDivClass').hide();
	  $('.nav-link').removeClass('active');
	  let newClasses = 'active ' + $(e.currentTarget).attr('class');
	  $(e.currentTarget).attr('class', newClasses);
	  let classlist = $(e.currentTarget).attr('class').split(' ');
	  let className = classlist[classlist.length - 1];
	  if($('.'+className).hasClass('d-none')) {
		$('.'+className).removeClass('d-none');
	  }
	  $('.'+className).show();
	});
});
    