<template>
    <nav class="navbar">
      <ul>
        <li><router-link to="/">Home</router-link></li>
        <li><router-link to="/products">Products</router-link></li>
    
        <li v-if="categories.length" class="dropdown">
          <span @click="toggleCategories" class="dropdown-link">Kategorier</span>
          <ul ref="tree" @blur="closeModal" v-if="showCategories" class="dropdown-menu">
            <li v-for="category in categories" :key="category.id">
              <div @click="toggleSubcategories(category.id)" class="category-item">
                {{ category.name }}
              </div>
              <ul v-if="subcategoriesVisible[category.id]" class="subcategory-menu">
                <li v-if="category.subcategories.length === 0" class="no-subcategories">
                  Inga subkategorier
                </li>
                <li v-for="sub in category.subcategories" :key="sub.id">
                  <router-link :to="'/SubCatView/' + sub.id">
                    {{ sub.name }}
                  </router-link>
                </li>
              </ul>
            </li>
          </ul>
        </li>
        <li><router-link to="/about">About Us</router-link></li>
    
        <SearchBox
          @searchQuery="(searchQuery) => $emit('searchQuery', searchQuery)"
          :clearSearch="clearSearch" 
          />
    
        <div class="IconClass">
          
          <li>
  <router-link  to="/wishlist" class="auth-link fa fa-heart">
  </router-link>
          </li>
  
          <li>
            <router-link to="/cart" class="cart-link fa fa-shopping-cart">
              <span> Cart</span>
              <span v-if="cartCount > 0" class="cart-count">{{ cartCount }}</span>
            </router-link>
          </li>
    
          <li>
            <router-link v-if="!isLoggedIn" to="/login" class="auth-link fa fa-user">
              <span> Login</span>
            </router-link>
            <router-link v-else to="/profile" class="auth link fa fa-user">
              <span> Profil</span>
            </router-link>
          </li>
        </div>
      </ul>
    </nav>
  </template>
  
  <script>
  import SearchBox from '../components/SearchBox.vue';
  import { mapGetters } from 'vuex';
  
  export default {
    name: 'NavbarComp',
    components: {
      SearchBox,
    },
    props: {
      showCategories: Boolean,
      clearSearch:Boolean, 
      searchQuery: {
        type: String,
        required: true,
      },
    },
    data() {
      return {
        categories: [],
        subcategoriesVisible: {},
        localSearchQuery: this.searchQuery,
      };
    },
    computed: {
      ...mapGetters(['cartCount', 'isLoggedIn']),
    },
    methods: {
      
      updateSearchQuery(value) {
        this.localSearchQuery = value;
        this.$emit('update:searchQuery', value);
      },
      toggleCategories() {
        this.$emit('toggle-categories');
      },
      toggleSubcategories(categoryId) {
        if (!this.subcategoriesVisible[categoryId]) {
          this.fetchSubcategories(categoryId);
        }
        this.subcategoriesVisible[categoryId] = !this.subcategoriesVisible[categoryId];
      },
      async fetchCategories() {
        try {
          const response = await fetch('https://localhost:7131/api/categories');
          const data = await response.json();
    
          this.categories = data.map((item) => ({
            id: item.category.id,
            name: item.category.name,
            subcategories: [],
          }));
        } catch (error) {
          console.error('Fel vid hämtning av kategorier:', error);
        }
      },
      async fetchSubcategories(categoryId) {
        try {
          const response = await fetch(
            `https://localhost:7131/api/categories/${categoryId}/subcategories`
          );
          const data = await response.json();
    
          const category = this.categories.find((cat) => cat.id === categoryId);
          if (category) {
            category.subcategories = data;
          }
        } catch (error) {
          console.error(`Fel vid hämtning av subkategorier för kategori ${categoryId}:`, error);
        }
      },
    },
    created() {
      this.fetchCategories();
      this.$store.dispatch('checkTokenExpiration');
    
      setInterval(() => {
        this.$store.dispatch('checkTokenExpiration');
      }, 30 * 60 * 1000); // 30 minuter
    },
  };
  </script>
  
  

<style scoped>
span{font-family: 'Poppins', sans-serif;}

.IconClass{
    margin-left: 3rem;
    display: flex;
    gap: 10px;
}


.navbar ul {
  list-style: none;
  display: flex;
  gap: 20px;

}

.navbar ul li {
    
  font-size: 20px;
}

.navbar ul li a {
  text-decoration: none;
  color: #333;
  font-weight: bold;
  transition: color 0.3s ease;
}

.navbar ul li a:hover {
  color: #ff4081;
}

.cart-link {

  position: relative;
  color: white;
  padding: 5px 10px;
  border-radius: 4px;
}


.cart-count {
  position: absolute;
  top: -5px;
  right: -5px;
  background-color: rgba(255, 255, 255, 0.5); 
  color: #fc6e4a;
  border-radius: 50%;
  padding: 2px 8px;
  font-size: 12px;
}

/* Stilar för kategoridropdown */
.dropdown {
  position: relative;
}

.dropdown-link {
  cursor: pointer;
  font-weight: bold;
  color: #333;
  transition: color 0.3s ease;
}

.dropdown-link:hover {
  color: #ff4081;
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  background-color: #fff;
  border: 1px solid #ccc;
  border-radius: 4px;
  list-style: none;
  padding: 10px 0;
  min-height: 100%;
  max-width: 300px; 
  min-width: 300px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  display: none; /* Initialt dolda, vi använder JavaScript eller fokusering för att visa den */
  z-index: 10; /* Gör så att den visas ovanpå andra element */
}

.dropdown-menu li {
  padding: 10px 20px;
}

.dropdown-menu li a {
  text-decoration: none;
  color: #333;
  display: block;
}

.dropdown-menu li a:hover {
  background-color: #f1f1f1;
}

/* När man hovrar över huvudkategorin, visa kategorierna */
.dropdown:hover .dropdown-menu,
.dropdown:focus-within .dropdown-menu {
  display: block;
}

.subcategory-menu {
  padding-left: 20px;
  list-style: disc;
  display: block;
  max-height: 500px;
  overflow-y: auto;
}

.subcategory-menu li {
  margin-bottom: 10px;
}

.no-subcategories {
  color: #999;
  font-style: italic;
}

/* Fokuserad dropdown (behåll synlig efter klick) */
.dropdown:focus-within .dropdown-menu {
    max-width: 300px; 
  min-width: 300px;
}

</style>
