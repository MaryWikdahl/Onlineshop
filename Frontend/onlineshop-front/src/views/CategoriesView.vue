<template>
    <div class="categories-view">
      <h1>Category and Subcategories</h1>
  
      <div v-if="loading" class="loading">Loading...</div>
      <div v-if="error" class="error">{{ error }}</div>
  
      <div v-if="subcategories.length > 0" class="subcategories">
        <h2>Subcategories for Category {{ categoryId }}</h2>
        <ul>
          <li v-for="subcategory in subcategories" :key="subcategory.id">
            {{ subcategory.name }}
          </li>
        </ul>
      </div>
  
      <div v-else-if="!loading && !error">
        <p>No subcategories found for this category.</p>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        categoryId: null, // Retrieved from URL
        subcategories: [], // Fetched subcategories
        loading: false, // Loading state
        error: null, // Error message
      };
    },
    created() {
      // Fetch categoryId from the route parameters

      this.fetchSubcategories();
      
    },
    methods: {
      async fetchSubcategories() {
        this.loading = true;
        this.error = null;
        this.categoryId = this.$route.params.categoryId;
        try {
          const response = await fetch(`https://localhost:7131/api/categories/${this.categoryId}/subcategories`);
          if (!response.ok) {
            throw new Error(`Error fetching data: ${response.statusText}`);
          }
          const data = await response.json();
          this.subcategories = data;
        } catch (err) {
          this.error = err.message;
        } finally {
          this.loading = false;
        }
      },
    },
  };
  </script>
  
  <style scoped>
  .categories-view {
    max-width: 600px;
    margin: 0 auto;
    padding: 20px;
    font-family: Arial, sans-serif;
  }
  
  .loading {
    color: #007bff;
    font-weight: bold;
  }
  
  .error {
    color: red;
    font-weight: bold;
  }
  
  .subcategories ul {
    list-style-type: none;
    padding: 0;
  }
  
  .subcategories li {
    padding: 5px 0;
    border-bottom: 1px solid #ddd;
  }
  </style>
  