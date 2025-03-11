<template>
     <div class="profile-header">
      <h1>Min Profil</h1>
    </div>
  <div class="profile-container">
 

    <div v-if="isLoggedIn && user" class="profile-content">
   

      <!-- Profilinformation -->
      <main class="profile-main">
        <div class="profile-card">
          <div class="profile-info">
            <img src="https://via.placeholder.com/150" alt="Profilbild" class="profile-picture" />
            <div>
              <h2>{{ user.name }}</h2>
              <p>{{ user.email }}</p>
            </div>
          </div>
          <div class="profile-details">
            <p><strong>Adress:</strong> {{ user.address }}</p>
            <p><strong>Stad:</strong> {{ user.city }}</p>
            <p><strong>Postnummer:</strong> {{ user.zipcode }}</p>
            <p><strong>Telefon:</strong> {{ user.mobile }}</p>
          </div>
        </div>
      </main>
    </div>
    

    <p v-else>Du är inte inloggad.</p>

    <div v-if="loading" class="loading-message">Laddar...</div>
    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
    <!-- Sidomeny -->
    <aside class="sidebar">
        <ul>
          <li>
            <router-link to="/edituser">Kontoinställningar</router-link>
          </li>
          <li>
          <router-link v-if="isAdmin" to="/AdminOrders">Redigera Ordrar</router-link>
          <router-link v-else to="/orders">Mina Ordrar</router-link>
        </li>
        <li>
            <router-link to="/wishlist">Min WishList</router-link>
          </li>
          <li>
            <router-link to="/changepassword">Byt Lösenord</router-link>
          </li>
         
          <li>
            <router-link to="/">Mitt medlemskap</router-link>
          </li>
          <li v-if="isAdmin">
            <router-link to="/AdminUser">Redigera Användare(users)</router-link>
          </li>
          <li v-if="isAdmin">
            <router-link to="/AdminProdukt">Admin Produkt Kontroll</router-link>
          </li>
          <li v-if="isAdmin">
            <router-link to="/AdminCategory">Admin Kategori Kontroll</router-link>
          </li>
          <li>
            <a @click="handleLogout">Logga ut</a>
          </li>
        </ul>
      </aside>
 
  </div>

</template>

<script>
import { mapState, mapActions, mapGetters } from 'vuex';

export default {
  name: 'ProfileView',

  computed: {
    ...mapState({
      user: state => state.user,
      loading: state => state.loading,
      errorMessage: state => state.errorMessage,
    }),
    ...mapGetters(['isAdmin']),
  },

  methods: {
    ...mapActions(['logout', 'fetchUser', 'isLoggedIn']),

    handleLogout() {
      this.logout(); // Logga ut användaren via Vuex
      this.$router.push('/login'); // Navigera till login-sidan
    },
  },

  created() {
    if (!this.isLoggedIn) {
      this.$router.push('/login'); // Skicka användaren till login om ej inloggad
    } else if (this.$store.state.token) {
      this.fetchUser(); // Hämta användarinformation om token finns
    }
     // Kontrollera tokenens giltighet direkt när komponenten laddas
  this.$store.dispatch('checkTokenExpiration');

// Starta en timer för att kontrollera tokenens utgång varje 30:e minut
setInterval(() => {
  this.$store.dispatch('checkTokenExpiration');
}, 30 * 60 * 1000); // 30 minuter i millisekunder
  },
  mounted() {
    console.log('Is admin:', this.isAdmin); // Logga för att se om du får rätt värde
  },
};
</script>

<style scoped>
/* Grundlayout */
.profile-container {
  max-width: 1200px;
  margin: 2rem auto;
  display: flex;
  gap: 2rem;
  background-color: #fff;
  padding: 1.5rem;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

/* Header */
.profile-header {


}

/* Sidomeny */
.sidebar {
  margin-top: 5rem;
  flex: 1;
  padding: 1rem;
}

.sidebar ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.sidebar li {
  margin-bottom: 1rem;
}

.sidebar a {
  text-decoration: none;
  font-weight: bold;
}

.sidebar a:hover {
  text-decoration: underline;
}

/* Profilinnehåll */
.profile-main {
 
  flex: 3;
  padding: 1rem;
}

.profile-card {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 1.5rem;

}

.profile-info {
  display: flex;
  align-items: center;
  gap: 1.5rem;

}

.profile-picture {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #ddd;
}

.profile-info h2 {
  margin: 0;
  font-size: 1.5rem;
  color: #333;
}

.profile-info p {
  margin: 0;
  color: #555;
}

.profile-details p {
  padding-left: 10rem;
  margin: 0.5rem 0;
  color: #555;
}

/* Felmeddelanden och laddning */
.loading-message {
  text-align: center;
  color: #555;
  font-size: 1rem;
  margin-top: 1rem;
}

.error-message {
  text-align: center;
  color: red;
  font-weight: bold;
  margin-top: 1rem;
}
</style>
