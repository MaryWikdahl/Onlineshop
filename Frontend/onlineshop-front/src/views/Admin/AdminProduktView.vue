<template>
    <div v-if="isAdmin">
      <h1>Admin Produkt Panel</h1>

  <AdminProducts/>

    </div>
  </template>
  
  <script>
  import AdminProducts from "../../components/AdminProductsComp.vue" ;

  import { mapGetters } from 'vuex';
  
  export default {
  name:"AdminView",
  components: {

      AdminProducts,

    },
    computed: {
    
    ...mapGetters(['isAdmin']),
  },

   
};

</script>