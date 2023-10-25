// Using jQuery
$(document).on('click', '.nav_icons', function() {
    $(this).toggleClass('nav__icons');
  });

  //Get the button
let mybutton = document.getElementById("btn-back-to-top");

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function () {
  scrollFunction();
};

function scrollFunction() {
  if (
    document.body.scrollTop > 20 ||
    document.documentElement.scrollTop > 20
  ) {
    mybutton.style.display = "block";
  } else {
    mybutton.style.display = "none";
  }
}
// When the user clicks on the button, scroll to the top of the document
mybutton.addEventListener("click", backToTop);

function backToTop() {
  document.body.scrollTop = 0;
  document.documentElement.scrollTop = 0;
}

$(document).ready(function () {

    const playVideoElement = document.querySelector('#play-video');

    if (playVideoElement) {
        playVideoElement.addEventListener('click', ev => ev.target.style.display = 'none');
    } else {
        console.log("No Play Video")
    }

  //document.querySelector('#play-video').addEventListener('click', ev => ev.target.style.display = 'none');

    var hasVideo = $('#play-video').length > 0;
    if (hasVideo == false) {
        return;
    } else {
        $('#play-video').on('click', function (e) {
            //the video id
            $('#video')[0].src += "?&autoplay=1";
            e.preventDefault();
        });
    }
    //get the video src url
    //var youTubeUrl = $('#video').attr('src');
    //the play / start button
    
});

