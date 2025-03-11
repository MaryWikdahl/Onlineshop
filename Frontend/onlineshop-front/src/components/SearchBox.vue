<template>
  <div class="search-container">
    <input 
      type="text" 
      v-model="localSearchQuery" 
      @input="() => this.$emit('searchQuery', localSearchQuery)" 
      placeholder="Search for product..." 
      
    />
    <i class="fa fa-search"></i>
  </div>
</template>

<script>
export default {
  name: 'SearchBox',
  props: {
    clearSearch: {
      type: Boolean,
      required:false
    },
  },
  mounted() {

  },
  watch: {
    
    clearSearch(newVal) {
      if (newVal) { 
       
        this.localSearchQuery = ''; 
        
      }
    },
  },
  methods: {
  
  },
  data() {
    return {
      localSearchQuery: '',
    };
  },
};
</script>


<style scoped>
.search-container {
  color: var(--mainblue) !important;
  margin-bottom: 1rem;
  display: flex;
}

input[type="text"] {
  background-color:#e5377118;
  padding: 0.5rem;
  width: 100%;
  color: var(--mainblue) !important;
  max-width: 300px;
  border: none;
  border-radius: 5px;
  box-sizing: border-box;
}
input:focus {
  background-color: #e5377186;
  border: none !important;
  outline: none;
}
.fa-search {
  margin-left: -30px !important;
  margin-top: 5px;
}
</style>
