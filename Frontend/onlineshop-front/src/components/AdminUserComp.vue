<template>
  <div v-if="isAdmin" class="admin-container">
    <!-- Ladda användare -->
    <button class="main-button" @click="() => { fetchUsers(); IsTrue = !IsTrue }">
  {{ IsTrue ? "Dölj användare" : "Ladda användare" }}
</button>
    <p v-if="loading" class="loading-message">Laddar användare...</p>
      <!-- Search for Specific Order -->
   <div>
      <input
        v-model="searchUserId"
        type="number"
        placeholder="Search Order by ID"
      />
      <button @click="fetchUserById">Search</button>
    </div>
<!-- Sortering och paginering --> 
<div v-if="users.length && IsTrue" class="controls">
  <label for="sortOrder">Sortera:</label>
  <select v-model="sortOrder" @change="sortAndPaginate">
    <option value="desc">Senast inlagd</option>
    <option value="asc">Äldst först</option>
  </select>
</div>
  <!-- Tabell för sökt användare -->
  <table v-if="foundUser" class="user-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Namn</th>
          <th>Email</th>
          <th>Adminstatus</th>
          <th>Åtgärder</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>{{ foundUser.id }}</td>
          <td>{{ foundUser.name }}</td>
          <td>{{ foundUser.email }}</td>
          <td>{{ foundUser.isAdmin ? "Ja" : "Nej" }}</td>
          <td>
            <button class="action-button delete-button" @click="deleteUser(foundUser.id)">Ta bort</button>
            <button class="action-button edit-button" @click="openEditModal(foundUser)">Ändra användare</button>
          </td>
        </tr>
      </tbody>
    </table>
    <!-- Tabell med användare -->
    <table v-if="users.length && IsTrue" class="user-table">

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
          <th>Email</th>
          <th>Adminstatus</th>
          <th>Åtgärder</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in  paginatedUsers " :key="user.id">
          <td>{{ user.id }}</td>
          <td>{{ user.name }}</td>
          <td>{{ user.email }}</td>
          <td>{{ user.isAdmin ? "Ja" : "Nej" }}</td>
          <td>
            <button class="action-button delete-button" @click="deleteUser(user.id)">Ta bort</button>
            <button class="action-button edit-button" @click="openEditModal(user)">Ändra användare</button>
          </td>
        </tr>
      </tbody>
    </table>

    <p v-else-if="!loading">Inga användare att visa.</p>

    <!-- Modal för att redigera användare -->
    <div v-if="showEditModal" class="modal">
      <div class="modal-content">
        <h2>Redigera användare</h2>
        <form @submit.prevent="updateUser(selectedUser.id)">
          <label for="name">Namn:</label>
          <input v-model="selectedUser.name" type="text" id="name" required />

          <label for="email">Email:</label>
          <input v-model="selectedUser.email" type="email" id="email" required />

          <label for="address">Adress:</label>
          <input v-model="selectedUser.address" type="text" id="address" />

          <label for="mobile">Mobil:</label>
          <input v-model="selectedUser.mobile" type="text" id="mobile" />

          <label for="city">Stad:</label>
          <input v-model="selectedUser.city" type="text" id="city" />

          <label for="zipcode">Postnummer:</label>
          <input v-model="selectedUser.zipcode" type="text" id="zipcode" />

          <div class="modal-actions">
            <button class="modal-button save-button" type="submit">Spara ändringar</button>
            <button class="modal-button cancel-button" type="button" @click="closeEditModal">Avbryt</button>
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
        users: [], 
        loading: false,
        showEditModal: false,
        searchUserId: '', // Användarens ID att söka efter
    foundUser: null,
        selectedUser: {
           name: '',
           email: '',
          address: '',
          mobile: '',
           city: '',
          zipcode: '',
          },
        IsTrue: false,
        paginatedUsers : [],
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
      async fetchUserById() {
  if (!this.searchUserId) {
    alert("Ange ett ID för att söka.");
    return;
  }

  const user = this.users.find(user => user.id === Number(this.searchUserId));

  if (!user) {
    alert("Ingen användare hittades med detta ID.");
    this.foundUser = null;
  } else {
    this.foundUser = user;
  }
}
,

      async fetchUsers() {
        this.loading = true;
        const token = this.$store.state.token;
        try {
          const response = await fetch("https://localhost:7131/api/users", {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          });
          if (!response.ok) {
            throw new Error("Kunde inte ladda användare.");
          }
   
          this.users = await response.json();
          this.totalPages = Math.ceil(this.users.length / this.pageSize);
          this.sortAndPaginate();
        } catch (error) {
          console.error(error.message);
        } finally {
          this.loading = false;
        }
      },

      sortAndPaginate() {
  // Sortera produkter baserat på id
  let sortedUsers = [...this.users];
  sortedUsers.sort((a, b) => {
    return this.sortOrder === "desc" ? b.id - a.id : a.id - b.id;
  });

  // Paginera produkter
  const start = (this.currentPage - 1) * this.pageSize;
  const end = start + this.pageSize;
  this.paginatedUsers = sortedUsers.slice(start, end);
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

      async deleteUser(userId) {
        const token = this.$store.state.token;
        try {
          const response = await fetch(`https://localhost:7131/api/users/${userId}`, {
            method: "DELETE",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          });
          if (!response.ok) {
            throw new Error("Kunde inte ta bort användaren.");
          }
          else{
            alert('User med id'+ ' ' + userId +' '+ 'är nu raderad')
          }
          this.users = this.users.filter((user) => user.id !== userId);
        } catch (error) {
          console.error(error.message);
        }
      },
      openEditModal(user) {
        this.selectedUser = { ...user };
        this.showEditModal = true;
      },
      closeEditModal() {
        this.showEditModal = false;
        this.selectedUser = null;
      },
      async updateUser(itemId) {
      const token = this.$store.state.token;
      try {
        const response = await fetch(`https://localhost:7131/api/Users/profile/${itemId}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(this.selectedUser),
        });

        if (!response.ok) {
          const errorText = await response.text();  // Läs felet som text om det inte är OK
          throw new Error(errorText);  // Kasta ett fel om status inte är OK
        }

        // Här väntar vi nu på JSON-svaret, eftersom servern returnerar ett JSON-objekt
        const data = await response.json();  // Läs svaret som JSON
        console.log("Svar från servern:", data);

        // Uppdatera användaren i listan med de nya uppgifterna
        const updatedUser = { ...this.selectedUser, id: itemId };  // Skapa ett objekt för den uppdaterade användaren
        const index = this.users.findIndex((u) => u.id === itemId);
        if (index !== -1) {
          this.users.splice(index, 1, updatedUser);
        }

        this.closeEditModal();  // Stäng redigeringsmodalen
      } catch (error) {
        console.error(error.message);  // Logga eventuella fel
        alert("Ett fel inträffade vid uppdatering av användaren.");
      }
}

},
mounted() {
  this.fetchUsers();
}
  };
  </script>

<style scoped>
/* Admin-container */
.admin-container {
  padding: 2rem;

}

/* Laddar användare */
.loading-message {
  color: #333;
  font-style: italic;
  margin: 1rem 0;
}

/* Tabell */
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

.user-table td:last-child {
  display: flex;
  gap: 0.5rem;
}

/* Knappar */
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

/* Modal */
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

.modal-content input {
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

/* Ingen behörighet */
.no-access {
  padding: 2rem;
  text-align: center;
  color: #777;
}
</style>
