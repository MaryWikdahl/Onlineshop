<template>
    <div class="admin-container" v-if="isAdmin">
      <button class="main-button" @click="() => { fetchCategories(); IsTrue = !IsTrue }">Ladda kategorier</button>
  
      <p v-if="loadingCategories"  class="loading-message" >Laddar kategorier...</p>
      <p v-else>Inga kategorier att visa.</p>

      
      <table v-if="categories.length && IsTrue " class="user-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Namn</th>
            <th>Subkategorier</th>
            <th>Åtgärder</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="category in categories" :key="category.id">
            <td>{{ category.id }}</td>
            <td>{{ category.name }}</td>
            
            <td>
              <ul>
                <li v-for="sub in category.subcategories" :key="sub.id">
                  {{ sub.name }}
                  {{ sub.id }}
                </li>
              </ul>
            </td>
            <td>
              <button class="action-button delete-button" @click="confirmDeleteCategory(category.id)">Ta bort</button>
              <button class="action-button delete-button" @click="openEditCategoryModal(category)">Ändra kategori</button>
            </td>
          </tr>
        </tbody>
      </table>
  
    
  
      <!-- Lägg till kategori och subkategori -->
      <div class="action-buttons">
        <button class="main-button" @click="openAddCategoryModal">Lägg till kategori</button>
        <button class="main-button" @click="openAddSubcategoryModal">Lägg till subkategori</button>
      </div>
  
      <!-- Modal för att redigera/lägga till kategori -->
      <div v-if="showCategoryModal" class="modal">
        <div class="modal-content">
          <h2>{{ editMode ? "Redigera kategori" : "Lägg till kategori" }}</h2>
          <form @submit.prevent="editMode ? updateCategory() : addCategory()">
            <label for="name">Namn:</label>
            <input v-model="selectedCategory.name" type="text" id="name" required />
  
            <div class="modal-actions">
              <button class="modal-button save-button"  type="submit">{{ editMode ? "Spara ändringar" : "Lägg till" }}</button>
              <button class="modal-button cancel-button"  type="button" @click="closeCategoryModal">Avbryt</button>
            </div>
          </form>
        </div>
      </div>
  
      <!-- Modal för att lägga till subkategori -->
      <div v-if="showCategoryModal" class="modal">
  <div class="modal-content">
    <h2>{{ editMode ? "Redigera kategori" : "Lägg till kategori" }}</h2>
    <form @submit.prevent="editMode ? updateCategory() : addCategory()">
      <label for="name">Kategori Namn:</label>
      <input v-model="selectedCategory.name" type="text" id="name" required />
      
      <h3>Subkategorier</h3>
      <div v-for="sub in selectedCategory.subcategories" :key="sub.id" class="subcategory-edit">
  <label :for="'subcategoryName' + sub.id">Subkategori Namn:</label>
  <input v-model="sub.name" :id="'subcategoryName' + sub.id" type="text" required />
  <button class="action-button delete-button" @click="deleteSubcategory(sub.id)">Ta bort Subkategori</button>

</div>

      <div class="modal-actions">
        <button class="modal-button save-button" type="submit">{{ editMode ? "Spara ändringar" : "Lägg till" }}</button>
        <button class="modal-button cancel-button" type="button" @click="closeCategoryModal">Avbryt</button>
      </div>
    </form>
  </div>
</div>
<!-- Lägg till subkategori -->
<div v-if="showSubcategoryModal" class="modal">
  <div class="modal-content">
    <h2>Lägg till subkategori</h2>
    <form @submit.prevent="addSubcategory">
      <!-- Välj kategori -->
      <label for="category">Välj kategori:</label>
      <select v-model="selectedSubcategory.categoryId" id="category" required>
        <option v-for="category in categories" :key="category.id" :value="category.id">
          {{ category.name }}
        </option>
      </select>
      
      <!-- Subkategori namn -->
      <label for="subcategoryName">Subkategori Namn:</label>
      <input v-model="selectedSubcategory.name" type="text" id="subcategoryName" required />
      
      <div class="modal-actions">
        <button class="modal-button save-button" type="submit">Lägg till</button>
        <button class="modal-button cancel-button" type="button" @click="closeSubcategoryModal">Avbryt</button>
      </div>
    </form>
  </div>
</div>


    </div>
  
    <div class="no-access" v-else>
      <p>Du har inte behörighet att visa den här sidan.</p>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        categories: [],
        loadingCategories: false,
        showCategoryModal: false,
        selectedCategory: null,
        editMode: false,
        showSubcategoryModal: false,
        selectedSubcategory: { name: "", categoryId: null },
        IsTrue: false,
        subcategoriesVisible: {},
      };
    },
    computed: {
      isAdmin() {
        return this.$store.getters.isAdmin;
      },
    },
    methods: {
      async deleteSubcategory(subcategoryId) {
  const token = this.$store.state.token;
  try {
    const response = await fetch(`https://localhost:7131/api/categories/${this.selectedCategory.id}/subcategories/${subcategoryId}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      console.error('API-fel vid borttagning av subkategori:', errorData);
      throw new Error('Kunde inte ta bort subkategori.');
    }

    // Ta bort subkategorin från listan lokalt i gränssnittet
    this.selectedCategory.subcategories = this.selectedCategory.subcategories.filter(sub => sub.id !== subcategoryId);
    alert('Subkategori borttagen!');
  } catch (error) {
    console.error('Fel vid borttagning av subkategori:', error.message);
    alert('Fel vid borttagning av subkategori.');
  }
}
,

async fetchCategories() {
  try {
    const response = await fetch('https://localhost:7131/api/categories');
    const data = await response.json();
    
    // console.log('API-svar:', data);

    this.categories = data.map(item => ({
      id: item.category.id,
      name: item.category.name,
      subcategories: [],  // Förbered för att hämta subkategorier
    }));

    // Hämta subkategorier för varje kategori
    this.categories.forEach(category => {
      this.fetchSubcategories(category.id);  // Hämtar subkategorier för varje kategori
    });

    // console.log('Uppdaterade kategorier:', this.categories);
  } catch (error) {
    console.error('Fel vid hämtning av kategorier:', error);
  }
},


async fetchSubcategories(categoryId) {
  try {
    const response = await fetch(`https://localhost:7131/api/categories/${categoryId}/subcategories`);
    const data = await response.json(); 

    // console.log(`Subkategorier för kategori ${categoryId}:`, data);

    const category = this.categories.find(cat => cat.id === categoryId);  
    if (category) {
      category.subcategories = data;
      // console.log(`Uppdaterad kategori med subkategorier:`, category);
    } else {
      console.log(`Kunde inte hitta kategori med id ${categoryId}`);
    }
  } catch (error) {
    console.error(`Fel vid hämtning av subkategorier för kategori ${categoryId}:`, error);
  }
},

      toggleSubcategories(categoryId) {
        if (!this.subcategoriesVisible[categoryId]) {
          this.fetchSubcategories(categoryId);
        }
        this.subcategoriesVisible[categoryId] = !this.subcategoriesVisible[categoryId];
      },
async addCategory() {
  const token = this.$store.state.token;
  
  // Se till att subcategories alltid finns i kategorin (även om det är en tom lista)
  const categoryPayload = {
    name: this.selectedCategory.name,
    subcategories: []  // Skicka alltid en tom lista om inga subkategorier ska läggas till
  };

  try {
    const response = await fetch('https://localhost:7131/api/categories', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(categoryPayload),
    });
    
    if (!response.ok) {
      throw new Error('Kunde inte lägga till kategori.');
    }

    const newCategory = await response.json();
    this.categories.push(newCategory);  // Lägg till den nya kategorin i listan
    this.closeCategoryModal();  // Stäng modal
  } catch (error) {
    console.error('Fel vid kategori tillägg:', error.message);
  }
},
async updateSubCategory(token) {
// Logga subkategorier som ska uppdateras
const subcategoriesCopy = this.selectedCategory.subcategories;

console.log('Subcategories to update:', subcategoriesCopy);

// Uppdatera subkategorier, en efter en
for (const subcategory of subcategoriesCopy) {
    // Kontrollera om subkategori är ogiltig
    if (!subcategory || !subcategory.id) {
        console.error('Ogiltig subkategori:', subcategory);
        continue; // Hoppa över ogiltiga subkategorier
    }

    const subcategoryToUpdate = {
        id: subcategory.id,
        name: subcategory.name,
        categoryId: this.selectedCategory.id // Håll subkategorins kategori-id intakt
    };

    console.log('Updating Subcategory:', subcategoryToUpdate);

    const subcategoryResponse = await fetch(`https://localhost:7131/api/categories/${this.selectedCategory.id}/subcategories/${subcategory.id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(subcategoryToUpdate),
    });

    console.log('Subcategory Update Response:', subcategoryResponse);

    if (!subcategoryResponse.ok) {
        const errorData = await subcategoryResponse.text();
        console.error('API-fel vid uppdatering av subkategori:', errorData);
        throw new Error('Kunde inte uppdatera subkategori.');
    }
}

},
async updateCategory() {
    const token = this.$store.state.token;

    try {
        // Kontrollera om selectedCategory finns
        if (!this.selectedCategory) {
            console.error('Fel: Ingen kategori valdes.');
            alert('Fel: Ingen kategori valdes.');
            return;
        }

        // Första: Uppdatera kategorin
        const categoryToUpdate = {
            ...this.selectedCategory,
            subcategories: Array.isArray(this.selectedCategory.subcategories) ? this.selectedCategory.subcategories : [], // Se till att subcategories är en array
        };

        // Skicka PUT-begäran för att uppdatera kategorin
        const categoryResponse = await fetch(`https://localhost:7131/api/categories/${this.selectedCategory.id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(categoryToUpdate),
        });

        console.log('Kategori Update Response:', categoryResponse);

        if (!categoryResponse.ok) {
            const errorData = await categoryResponse.text();
            console.error('API-fel vid uppdatering av kategori:', errorData);
            throw new Error('Kunde inte uppdatera kategori.');
        }

        // // Om kategorin uppdaterades (204 No Content)
        // if (categoryResponse.status === 204) {
        //     alert('Kategori uppdaterad!');
        // }

        await this.updateSubCategory(token);

        alert('Kategori och subkategori är nu uppdaterade!');
        this.closeCategoryModal();
        await this.fetchCategories();

    } catch (error) {
        console.error('Fel vid uppdatering av kategori eller subkategorinnnn:', error.message);
        alert('Fel vid uppdatering av kategori eller subkategori.');
    }
}


,
async deleteCategory(categoryId) {
    const token = this.$store.state.token;
    try {
        const response = await fetch(`https://localhost:7131/api/categories/${categoryId}`, {
            method: 'DELETE',
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });

        // Om svaret inte är OK, logga mer information
        if (!response.ok) {
            const errorData = await response.json(); // Hämta felmeddelandet från servern
            console.error('API-fel vid borttagning av kategori:', errorData);
            throw new Error('Kunde inte ta bort kategori.');
        }

        // Om borttagningen lyckades, ta bort kategorin från listan
        this.categories = this.categories.filter(category => category.id !== categoryId);
        console.log(`Kategori med id ${categoryId} har tagits bort`);
    } catch (error) {
        console.error('Fel vid borttagning av kategori:', error.message);
    }
}
,
  
      openAddCategoryModal() {
        this.selectedCategory = { name: '' };
        this.editMode = false;
        this.showCategoryModal = true;
      },
  
      openEditCategoryModal(category) {
        this.selectedCategory = { ...category };
        this.editMode = true;
        this.showCategoryModal = true;
      },
  
      closeCategoryModal() {
        this.showCategoryModal = false;
        this.selectedCategory = null;
      },
  
      openAddSubcategoryModal() {
        this.selectedSubcategory = { name: '', categoryId: null };

        this.showSubcategoryModal = true;
      },
  
      closeSubcategoryModal() {
        this.showSubcategoryModal = false;
        this.selectedSubcategory = { name: '', categoryId: null };
      },
    async confirmDeleteCategory(categoryId){
        const isConfirmed = window.confirm('Är du säker på att du vill ta bort denna kategori?');
    if (isConfirmed) {
      await this.deleteCategory(categoryId);
    }
    },
      async addSubcategory() {
        const token = this.$store.state.token;
        try {
          const response = await fetch(`https://localhost:7131/api/categories/${this.selectedSubcategory.categoryId}/subcategories`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(this.selectedSubcategory),
          });
          if (!response.ok) {
            throw new Error('Kunde inte lägga till subkategori.');
          }
          const newSubcategory = await response.json();
          const category = this.categories.find(c => c.id === this.selectedSubcategory.categoryId);
          category.subcategories.push(newSubcategory);
          this.closeSubcategoryModal();
        } catch (error) {
          console.error('Fel vid subkategori tillägg:', error.message);
        }
      },


    },
    
  };
  </script>
  <style scoped>
  /* Återanvändning av stilar från den första vyn */
  .admin-container {
    padding: 2rem;
  
  }
  
  .loading-message {
    color: #333;
    font-style: italic;
    margin: 1rem 0;
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
  
  .edit-button {
    background-color: var(--mainblue);
    color: white;
  }
  
  .edit-button:hover {
    background-color: darkblue;
  }
  
  .modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
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
  
  .modal-button {
    padding: 8px 15px;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  .save-button {
    background-color: var(--mainblue);
    color: white;
  }
  
  .save-button:hover {
    background-color: darkblue;
  }
  
  .cancel-button {
    background-color: #ff4d4d;
    color: white;
  }
  
  .cancel-button:hover {
    background-color: #e60000;
  }
  
  .no-access {
    padding: 2rem;
    text-align: center;
    color: #777;
  }
  </style>
  