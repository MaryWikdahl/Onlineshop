<template>
  
  <div id="app">
    <Header 
      ref="headerComp" 
      class="boxx" 
      :showCategories="showCategories" 
      @toggle-categories="toggleCategories" 
      :searchQuery="searchQuery" 
      @filter-Products="filterProducts" 
       @searchQuery="handleSearchQuery"
  
     
      :clearSearch="clearSearch" 
      
    />
    <div class="body">
      <!-- <ProdView 
        v-if="searchQuery.length > 0" 
        @item-clicked="(itemClicked, _id) => {isItemClicked = itemClicked; id = _id}"
        :searchQuery="searchQuery" 
      /> -->
      <router-view/>
    </div>
  </div>
</template>

<script>
import Header from "./components/HeaderComp.vue";
// import ProdView from "./views/ProductsView.vue";
import { mapGetters, mapActions } from "vuex";
export default {
  name: "App",
  components: {
    Header,
    // ProdView
  },
  data() {
    return {
      searchQuery: '', // prop till produkt vyn och emit från searchbox
      showCategories: false, // Kategori visning
      clearSearch: false, //Prop till searchbox för att tömma input
      isItemClicked :true,
      id: 0
    };
  },
  computed: {
    ...mapGetters(["isItemClicked"]), // Hämta clearSearch från Vuex
  
  },
  methods: {
    ...mapActions(["updateIsItemClicked"]),
    handleSearchQuery(query) {
    this.$store.dispatch("setSearchQuery", query); // Skicka sökfrågan till Vuex
    if (query.length > 0) {
      this.$router.push("/products"); // Navigera till produktvyn
    } else {
      this.$router.push("/"); // Gå tillbaka till startsidan om sökfrågan är tom
    }
  },
  clearSearchQuery() {

  this.$store.dispatch("setSearchQuery", ''); // Töm Vuex searchQuery
  this.searchQuery = ''; // Töm den lokala searchQuery

  // Vänta tills nästa render-cykel
  this.$nextTick(() => {
    // console.log("Tömning av searchQuery i nästa tick");
    this.updateIsItemClicked(true); // Uppdatera Vuex med true för isItemClicked
    setTimeout(() => {
      this.updateIsItemClicked(false); // Återställ isItemClicked till false efter 500ms
      console.log("Nu, isItemClicked:", this.isItemClicked);
    }, 500);
  });
},
    toggleCategories() {
      this.showCategories = !this.showCategories;
    },
    handleClickOutside(event) {
      setTimeout(() => {
      if (this.isItemClicked) {
      this.clearSearchQuery();
    }  }, 500);
   
    const headerElement = this.$refs.headerComp?.$el;
    if (!headerElement) return

    if (!headerElement.contains(event.target)) {
      this.showCategories = false; // Stäng kategorier
    }
  },

    handleMouseLeave() {
      this.showCategories = false; // Stäng kategoriträdet när muspekaren lämnar
    },
  },
  mounted() {
    this.$nextTick(() => {
      document.addEventListener("mousedown", this.handleClickOutside); // Lyssna på alla mus-klick
      const headerElement = this.$refs.headerComp?.$el;
      if (headerElement) {
        headerElement.addEventListener("mouseleave", this.handleMouseLeave); // För mus leave-event på headern
      }
    });
  },
  unmounted() {
    // Rensa event-lyssnare när komponenten tas bort från DOM
    document.removeEventListener("mousedown", this.handleClickOutside);
  },
};
</script>



<style>
#app {
  background-color: #fefaf9;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
.boxx { 
  position: relative;
  z-index: 10;
}
.body {
  padding-top: 3rem;
  min-height: 100rem;
  background-color: white;
  margin-left: 15rem;
  margin-right: 15rem;
}
</style>
