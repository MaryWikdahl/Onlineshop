<template>
  <div class="home">
   <Slideshow/>
   <h1 style="margin-top: 10rem;">Nyheter</h1>
  <Products/>
  </div>
</template>

<script>

import Slideshow from "../components/Slideshow.vue"
import Products from "../components/NewProductsComp.vue";

export default {
  name: 'HomeView',
 components:{
  Slideshow: Slideshow,
  Products
  
 }
}
</script>
