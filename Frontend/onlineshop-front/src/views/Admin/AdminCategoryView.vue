<template>
    <div v-if="isAdmin">
      <h1>Admin Kategori Panel</h1>
  <AdminCat/>
    </div>
  </template>
  
  <script>

  import AdminCat from "../../components/AdminCategoriesComp.vue" ;
  import { mapGetters } from 'vuex';
  
  export default {
  name:"AdminView",
  components: {

      AdminCat
    },
    computed: {
    
    ...mapGetters(['isAdmin']),
  },

   
};

</script>