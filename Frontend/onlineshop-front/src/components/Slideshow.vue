<template>
    <!-- Slideshow container -->
    <div class="slideshow-container">

    <!-- Full-width images with number and caption text -->
    <!-- första sliden -->
    <div v-show="slideIndex == 0" class="mySlides fade">
      <h1 class="unik-pic">Klänningar för festligheter</h1>
    
    <div class="text-span">
      <p style="width: 400%;">Jul</p>
      <p style="width: 340%;">Party</p>
      <p style="width: 340%;">Nyår</p>
    </div>

    <img class="pictures"  src="../assets/pictures/img1.jpg" >
    </div>
<!-- andra sliden -->
    <div v-show="slideIndex == 1" class="mySlides fade">
      <h1 class="leva-pic">Hitta din nyårslook här</h1>
      
    <img  class="pictures" src="../assets/pictures/img2.jpg" >
    </div>
<!-- tredje sliden -->
    <div v-show="slideIndex == 2" class="mySlides fade">
      <div class="text-span2">
        <p style="width: 500%;">Julklappar</p>
      <p style="width: 470%;">Mjukis set</p>
      <p style="width: 550%;">Stickade plagg</p>
      <p style="width: 600%;" >Lounge set</p></div>
    <img class="pictures" src="../assets/pictures/img3.jpg">
    </div>
    <!-- Next and previous buttons -->
    <a class="prev" @click="changeSlide(-1)">&#10094;</a>
    <a class="next" @click="changeSlide(1)">&#10095;</a>
    </div>
    <br>

    <!-- The dots/circles -->
    <div>
    <span class="dot" :class="slideIndex == 0 ? 'active' : ''" @click="slideIndex == 0"></span>    <!-- dålig praxis men fungerar likadant -->
    <span class="dot" :class="slideIndex == 1 ? 'active' : ''" @click="setSlide(1)"></span>
    <span class="dot" :class="slideIndex == 2 ? 'active' : ''" @click="setSlide(2)"></span>
    </div>
</template>
    <script>

export default{
    name:'Slideshow',
    data(){ 
        return {
            slideIndex: 0,
        }
    },
    methods: {
        changeSlide(slideNumber) {
            this.slideIndex += slideNumber;

            if(this.slideIndex < 0)
                this.slideIndex = 2;
            if(this.slideIndex > 2)
                this.slideIndex = 0;
        },
        setSlide(slideNumber) {
            this.slideIndex = slideNumber;
        }

    }
}
</script>

<style scoped>
   * {box-sizing:border-box}

/* Slideshow container */
.slideshow-container {
 margin-left: 2rem;
}
h1{
  color: white;
}

/* Next & previous buttons */
.prev, .next {
  left: 45rem;
  cursor: pointer;
  position: absolute;
  top: 50%;
  width: auto;
  margin-top: 21rem;
  padding: 46px;
  color: var(--mainblue);
  font-weight: bold;
  font-size: 38px;
  transition: 0.6s ease;
  border-radius: 0 3px 3px 0;
  user-select: none;
}

/* Position the "next button" to the right */
.next {
  right: 18rem;
  border-radius: 3px 0 0 3px;
}



/* The dots/bullets/indicators */
.dot {
  cursor: pointer;
  height: 15px;
 width: 15px;
 margin-left: 1rem;
 display: inline-block;
  background-color: #bbb;
  border-radius: 50%;
  transition: background-color 0.6s ease;
}

.active, .dot:hover {
  background-color: #717171;
}

/* Fading animation */
.fade {
  animation-name: fade;
  animation-duration: 1.5s;
}

@keyframes fade {
  from {opacity: .4}
  to {opacity: 1}
}
.pictures{
  box-shadow: 5px 5px 5px rgba(0, 0, 0, 0.3);


    border-radius: 1rem;
    height: 40rem;
    width: 30rem;
    margin-left: 4rem;
    margin-right: 4rem;
}
.text-span2{

  display: block;
  display: inline-block;
 position: absolute;
 top:35rem;
 right: 80rem;
 

}
.text-span{
  display: block;
  display: inline-block;
 position: absolute;
 top:32rem;
 right: 68rem;
 

}

 .unik-pic{ 
 position: absolute;
 left:48rem;
 top: 25rem;

 }
 .leva-pic{ 
 position: absolute;
 top: 25rem;
 left: 50rem;

 }
    </style>