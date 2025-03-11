<template>
  <header
   @mousedown="handleClickOutside"
  class="header">
    <div class="container">
      <li><router-link to="/">
          <img class="logo"  src="../assets/pictures/newlogo.png" alt="" >
      </router-link></li>
        <Navbar 
        @searchQuery="(searchQuery) => $emit('searchQuery', searchQuery)"   
         :clearSearch="clearSearch"   
         :showCategories="showCategories" 
         @toggle-categories="toggleCategories"
         
         />

         
    </div>
  </header>
</template>


<script>

import Navbar from  "../components/NavbarComp.vue" ;
export default {
  name: "HeaderComp",
  components: {
    Navbar
    },
  data() {
    return {
      PropBool: false
    };
  },
  props: {
    showCategories: Boolean, // Parent skickar detta ner
    clearSearch: Boolean, 
  },
  methods: {
 
    toggleCategories() {
      // this.showCategories = !this.showCategories; 
      this.$emit("toggle-categories");
    },
    async handleClickOutside(event) {
      const tree = this.$refs.tree; // Hämtar referensen
   if (tree && !tree.contains(event.target)) {
      // Om klicket är utanför trädet, stäng det
      await this.$emit("toggle-categories");
    }},
  
  },

};
</script>


<style scoped>

.header {
  z-index: 10;
  background-color: #fad7ce;
  box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.2);

  padding: 30px 0;
  height: 6rem;
  display: flex;
 
}

.container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;

  list-style: none;
}
.logo{
  
  height: 10rem;
margin-top:-2rem ;
  width: auto; /* Bevarar bildens proportioner */
}

</style>
