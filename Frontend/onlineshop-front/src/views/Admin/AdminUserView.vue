<template>
    <div v-if="isAdmin">
  <AdminUser/>
    </div>
  </template>
  
  <script>
  import AdminUser from "../../components/AdminUserComp.vue" ;

  import { mapGetters } from 'vuex';
  
  export default {
  name:"AdminUserView",
  components: {
      AdminUser,
  
    },
    computed: {
    
    ...mapGetters(['isAdmin']),
  },

   
};

</script>