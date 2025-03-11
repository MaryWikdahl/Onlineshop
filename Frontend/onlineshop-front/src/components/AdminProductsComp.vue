<template>
  <div v-if="isAdmin" class="admin-container">
    <!-- Laddar produkter -->
    <button class="main-button" @click="() => { fetchProducts(); IsTrue = !IsTrue }">
      {{ IsTrue ? "Dölj produkter" : "Ladda produkter" }}
    </button>
    <p v-if="loadingProducts" class="loading-message">Laddar produkter...</p>



<!-- Sortering och paginering --> 
<div v-if="products.length && IsTrue" class="controls">
  <label for="sortOrder">Sortera:</label>
  <select v-model="sortOrder" @change="sortAndPaginate">
    <option value="desc">Senast inlagd</option>
    <option value="asc">Äldst först</option>
  </select>
</div> 
    <!-- Tabell med produkter -->
    <table v-if="paginatedProducts.length && IsTrue" class="user-table">

        <!-- sidhantering -->
    <div class="pagination">
      <button class="action-button main-button" @click="changePage(currentPage - 1)" :disabled="currentPage === 1">
        Föregående
      </button>
    <span>Sida {{ currentPage }} av {{ totalPages }}</span>
    <button class="action-button main-button" @click="changePage(currentPage + 1)" :disabled="currentPage === totalPages">
        Nästa
    </button>
    </div>


      <thead>
        <tr>
          <th>ID</th>
          <th>Namn</th>
          <th>Beskrivning</th>
          <th>Pris</th>
          <th>Subkategori</th>
          <th>Produktinformation</th> 
          <th>Åtgärder</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="product in paginatedProducts" :key="product.id">
          <td>{{ product.id }}</td>
          <td>{{ product.name }}</td>
          <td class="description-cell">{{ product.description }}</td>
          <td>{{ product.price }}</td>
          <td>{{ product.subCategoryId }}</td>
          
          <!-- Kolumn för produktinfo -->
          <td>
            <table class="variant-table">
              <thead>
                <tr>
                  <th>Storlek</th>
                  <th>Lager</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="variant in product.productInfo" :key="variant.size">
                  <td>{{ variant.size }}</td>
                  <td>{{ variant.stockQuantity }}</td>
                </tr>
              </tbody>
            </table>
          </td>

          <td>
            <button class="action-button delete-button" @click="confirmDeleteProduct(product.id)">Ta bort</button>
            <button class="action-button main-button" @click="openEditProductModal(product)">Ändra produkt</button>
          </td>
        </tr>
      </tbody>
      
    </table>
    <p v-else-if="!loadingProducts">Inga produkter att visa.</p>

    <!-- Lägg till ny produkt -->
    <button class="main-button" @click="openAddProductModal">Lägg till produkt</button>

    <!-- Modal för att redigera/lägga till produkt -->
    <div v-if="showProductModal" class="modal">
      <div class="modal-content">
        <h2>{{ editMode ? "Redigera produkt" : "Lägg till produkt" }}</h2>
        <form @submit.prevent="editMode ? updateProduct() : addProduct()">
          <label for="name">Namn:</label>
          <input v-model="selectedProduct.name" type="text" id="name" required />

          <label for="description">Beskrivning:</label>
          <textarea v-model="selectedProduct.description" id="description" required></textarea>

          <label for="price">Pris:</label>
          <input v-model="selectedProduct.price" type="number" id="price" required />

          <label for="categoryId">Kategori:</label>
          <select v-model="selectedProduct.categoryId" id="categoryId" @change="fetchSubcategories" required>
            <option value="">Välj kategori</option>
            <option v-for="category in categories" :key="category.id" :value="category.id">
              {{ category.name }}
            </option>
          </select>

          <label for="subCategoryId">Subkategori:</label>
          <select v-model="selectedProduct.subCategoryId" id="subCategoryId" required>
            <option value="">Välj subkategori</option>
            <option v-for="subcategory in subcategories" :key="subcategory.id" :value="subcategory.id">
              {{ subcategory.name }}
            </option>
          </select>

          <label for="encodedImage">Bild (Base64):</label>
          <textarea v-model="selectedProduct.encodedImage" id="encodedImage" required class="encoded-image-textarea"></textarea>
          
          <button type="button" @click="sizeorcolor = !sizeorcolor" class="action-button main-button" style="margin-top:2rem;">
  <span v-if="sizeorcolor">Ändra till färg</span>
  <span v-else>Ändra till storlek</span>
</button>
          <!-- Hantera produktens varianter -->
<div v-for="item in selectedProduct.productInfo" :key="item.id">
  <h3>ProduktInfo</h3>
  <div class="variant-fields">
    <div v-if="sizeorcolor">
      <label for="'size-' + item.id">Storlek:</label>
      <input v-model="item.size" :id="'size-' + item.id" type="text" />
    </div>
    <div v-else>
      <label for="'color-' + item.id">Färg:</label>
      <input v-model="item.color" :id="'color-' + item.id" type="text" />
    </div>

    <label for="'stockQuantity-' + item.id">Lager:</label>
    <input v-model="item.stockQuantity" :id="'stockQuantity-' + item.id" type="number" required />

    <!-- Radera knapp -->
    <button
      type="button"
      @click="removeInfo(item)"
      v-if="selectedProduct.productInfo.length > 1"
      class="remove-btn"
    >
      <i class="fa fa-trash"></i>
    </button>
  </div>
</div>

<!-- Lägg till ProduktInfo -->
<button type="button" class="action-button main-button" @click="addProduktInfo">
  Lägg till ProduktInfo
</button>


          <div class="modal-actions">
            <button class="action-button main-button" type="submit">
              {{ editMode ? "Spara ändringar" : "Lägg till" }}
            </button>
            <button class="action-button delete-button" type="button" @click="closeProductModal">Avbryt</button>
          </div>
        </form>
      </div>
    </div>
  </div>

  <div v-else class="no-access">
    <p>Du har inte behörighet att visa den här sidan.</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      products: [],
      loadingProducts: false,
      showProductModal: false,
      selectedProduct: null,
      editMode: false,
      IsTrue: false,
      categories: [],
      subcategories: [],
      sizeorcolor: true,
      allProducts: [], 
      paginatedProducts: [],
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    sortOrder: "desc",
    };
  },
  computed: {
    isAdmin() {
      return this.$store.getters.isAdmin;
    },
  },
  methods: {
    async fetchCategories() {
      const token = this.$store.state.token;
      try {
        const response = await fetch("https://localhost:7131/api/categories", {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) {
          throw new Error("Kunde inte ladda kategorier.");
        }
        const data = await response.json();
        this.categories = data.map(item => item.category);
        console.log("Kategorier laddade:", this.categories);
      } catch (error) {
        console.error("Fel vid hämtning av kategorier:", error.message);
      }
    },
  
    async fetchSubcategories() {
      if (!this.selectedProduct.categoryId) {
        this.subcategories = [];
        return;
      }
      const token = this.$store.state.token;
      try {
        const response = await fetch(`https://localhost:7131/api/categories/${this.selectedProduct.categoryId}/subcategories`, {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) {
          throw new Error("Kunde inte ladda subkategorier.");
        }
        this.subcategories = await response.json();
        console.log("Subkategorier laddade:", this.subcategories);
      } catch (error) {
        console.error("Fel vid hämtning av subkategorier:", error.message);
      }
    },
  // hämtat produkter
   async fetchProducts() {
    this.loadingProducts = true;
    const token = this.$store.state.token;
    try {
      const response = await fetch("https://localhost:7131/api/products", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      if (!response.ok) {
        throw new Error("Kunde inte ladda produkter.");
      }
      this.products = await response.json();
      this.totalPages = Math.ceil(this.products.length / this.pageSize);
      this.sortAndPaginate();
    } catch (error) {
      console.error("Fel vid hämtning av produkter:", error.message);
    } finally {
      this.loadingProducts = false;
    }
  },
  sortAndPaginate() {
  // Sortera produkter baserat på id
  let sortedProducts = [...this.products];
  sortedProducts.sort((a, b) => {
    return this.sortOrder === "desc" ? b.id - a.id : a.id - b.id;
  });

  // Paginera produkter
  const start = (this.currentPage - 1) * this.pageSize;
  const end = start + this.pageSize;
  this.paginatedProducts = sortedProducts.slice(start, end);
},
  changePage(newPage) {
    if (newPage > 0 && newPage <= this.totalPages) {
      this.currentPage = newPage;
      this.sortAndPaginate();
    }
  },

  changeSortOrder(order) {
    this.sortOrder = order;
    this.sortAndPaginate();
  },


  
    confirmDeleteProduct(productId) {
      const isConfirmed = window.confirm("Är du säker på att du vill ta bort denna produkt?");
      if (isConfirmed) {
        this.deleteProduct(productId);
      }
    },
  
    async deleteProduct(productId) {
      const token = this.$store.state.token;
      try {
        const response = await fetch(`https://localhost:7131/api/products/${productId}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) {
          throw new Error("Kunde inte ta bort produkten.");
        }
        this.products = this.products.filter((product) => product.id !== productId || !response.ok);

      } catch (error) {
        console.error("Fel vid borttagning av produkt:", error.message);
      }
    },
  
    openEditProductModal(product) {
  this.selectedProduct = JSON.parse(JSON.stringify(product)); // Skapar en djup kopia av produktdata
  this.editMode = true;
  this.showProductModal = true;
  this.fetchCategories();
  this.fetchSubcategories(); 
},

  
    openAddProductModal() {
      this.selectedProduct = {
        name: "",
        description: "",
        price: 0,
        stockQuantity: 0,
        categoryId: "",
        subCategoryId: "",
        productInfo: [{ size: "", color: "", stockQuantity: 0 }],
        encodedImage: "",
      };
      this.editMode = false;
      this.showProductModal = true;
      this.fetchCategories();
    },
  
    closeProductModal() {
  this.showProductModal = false;
  this.selectedProduct = {}; // Rensar data för nästa användning
  this.editMode = false;
},
    async addProduct() {
      const token = this.$store.state.token;
      try {
        const response = await fetch("https://localhost:7131/api/products", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(this.selectedProduct),
        });
        if (!response.ok) {
          throw new Error("Kunde inte lägga till produkten.");
        }
        const newProduct = await response.json();
        this.products.push(newProduct);
        this.closeProductModal();
      } catch (error) {
        console.error("Fel vid tillägg av produkt:", error.message);
      }
    },
    async updateProduct() {
      console.log('update', this.selectedProduct);
  const token = this.$store.state.token;

  try {
    const response = await fetch(`https://localhost:7131/api/products/${this.selectedProduct.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(this.selectedProduct),
    });

    if (!response.ok) {
      throw new Error("Kunde inte uppdatera produkten.");
    }

    const updatedProduct = await response.json();
const index = this.products.findIndex((product) => product.id === updatedProduct.id);
if (index !== -1) {
  this.products[index] = updatedProduct;  // Uppdatera korrekt
}

    // Stäng modal
    this.closeProductModal();
  } catch (error) {
    console.error("Fel vid uppdatering av produkt:", error.message);
  }
},

addProduktInfo() {
  // Kontrollera om en variant med samma storlek och färg redan finns
  const exists = this.selectedProduct.productInfo.some(
    (variant) => variant.size === "" && variant.color === ""
  );
  if (!exists) {
    this.selectedProduct.productInfo.push({
      size: "",
      color: "",
      stockQuantity: 0,
    });
  }
},
  
removeInfo(item) {
  // Ta bort produktvariant
  this.selectedProduct.productInfo = this.selectedProduct.productInfo.filter(
    (info) => info !== item  // Jämför objektet direkt
  );
}



  },
  mounted() {

  this.fetchProducts();
}
  
};
</script>

<style scoped>

.admin-container {
  padding: 2rem;

}

.loading-message {
  color: #333;
  font-style: italic;
  margin: 1rem 0;
}
.description-cell {
  max-width: 200px; /* Justera bredden efter behov */
  max-height: 50px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.user-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1.5rem;
  background: white;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  border-radius: 5px;
  overflow: hidden;
}

.user-table th,
.user-table td {
  padding: 1rem;
  text-align: left;
  border-bottom: 1px solid #eaeaea;
}

.user-table th {
  background-color: var(--mainblue);
  color: white;
  font-weight: bold;
}

.user-table tr:hover {
  background-color: #f1f1f1;
}
.remove-btn {
   
   background-color: transparent;
   border: none;
   outline: none;
   cursor: pointer;
   font-size: 16px;
 }
.main-button {
  padding: 10px 20px;
  background-color: var(--mainblue);
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 1rem;
}

.main-button:hover {
  background-color: darkblue;
}

.action-button {
  padding: 5px 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 0.9rem;
}

.delete-button {
  background-color: #ff4d4d;
  color: white;
}

.delete-button:hover {
  background-color: #e60000;
}


.modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 80%; /* Du kan justera bredden efter behov */
  max-width: 600px; /* Maxbredden på modalen */
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  z-index: 1000; /* För att se till att modalen är ovanpå andra element */
  max-height: 90%; /* Sätt en maxhöjd på modalen */
  overflow-y: auto; /* Gör modalen scrollbar om innehållet är för stort */
}

.modal-content {
  background: white;
  padding: 2rem;
  border-radius: 5px;
  width: 400px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.modal-content h2 {
  margin-bottom: 1rem;
  color: var(--mainblue);
}

.modal-content form {
  display: flex;
  flex-direction: column;
}

.modal-content label {
  margin-top: 1rem;
}

.modal-content input,
.modal-content textarea,
.modal-content select {
  margin-top: 0.5rem;
 
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 5px;
}

.modal-actions {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}



.no-access {
  padding: 2rem;
  text-align: center;
  color: #777;
}
.variant-fields{
  display:flex;
  height: 4rem;
  max-height: 4rem;
  padding-bottom: 1rem;
  padding-left: 3rem;
}
.variant-fields input{
 max-height: 1rem;
 width: 4rem;
}
.variant-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
  background-color: #f9f9f9;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
  max-height: 150px; /* Justera höjden efter behov */
  overflow-y: auto;
  display: block;
}

.variant-table th,
.variant-table td {
  padding: 8px;
  text-align: left;
  border-bottom: 1px solid #eaeaea;
}

.variant-table th {
  background-color: #f0f0f0;
}

.variant-table tr:hover {
  background-color: #f1f1f1;
}

</style>
