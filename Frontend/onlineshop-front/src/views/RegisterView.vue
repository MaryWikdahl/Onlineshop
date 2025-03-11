<template>
    <div class="register-container">
      <h1>Register</h1>
  
      <!-- Visa felmeddelande om det finns något -->
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  
      <!-- Registreringsformulär -->
      <form @submit.prevent="handleRegister">
        <div>
          <label for="name">Name:</label>
          <input
            type="text"
            id="name"
            v-model="credentials.name"
            required
          />
        </div>
  
        <div>
          <label for="email">Email:</label>
          <input
            type="email"
            id="email"
            v-model="credentials.email"
            required
          />
        </div>
  
        <div>
          <label for="password">Password:</label>
          <input
            type="password"
            id="password"
            v-model="credentials.password"
            required
          />
        </div>
  
        <div>
          <label for="address">Address:</label>
          <input
            type="text"
            id="address"
            v-model="credentials.address"
            required
          />
        </div>
  
        <div>
          <label for="mobile">Mobile:</label>
          <input
            type="text"
            id="mobile"
            v-model="credentials.mobile"
            required
          />
        </div>
  
        <div>
          <label for="city">City:</label>
          <input
            type="text"
            id="city"
            v-model="credentials.city"
            required
          />
        </div>
  
        <div>
          <label for="zipcode">Zipcode:</label>
          <input
            type="text"
            id="zipcode"
            v-model="credentials.zipcode"
            required
          />
        </div>
  
        <button type="submit" :disabled="loading">
          {{ loading ? 'Registering...' : 'Register' }}
        </button>
      </form>
  
      <!-- Länk till inloggningssidan om användaren redan har ett konto -->
      <p>Already have an account? <router-link to="/login">Login here</router-link></p>
    </div>
  </template>
  
  <script>
  import { mapActions } from 'vuex';
  
  export default {
    name: 'RegisterView',
    data() {
      return {
        credentials: {
          name: '',
          email: '',
          password: '',
          address: '',
          mobile: '',
          city: '',
          zipcode: '',
        },
        errorMessage: '',
        loading: false, // För att hantera laddningsstatus
      };
    },
    methods: {
      ...mapActions(['register']), // Använd Vuex action för registrering
  
      async handleRegister() {
        this.errorMessage = '';
        this.loading = true;
  
        try {
          // Skicka registreringsdata till backend via Vuex action
          await this.register(this.credentials);
          this.$router.push('/login'); // Navigera till inloggningssidan efter lyckad registrering
        } catch (error) {
          this.errorMessage = error.message || 'An error occurred during registration.';
        } finally {
          this.loading = false;
        }
      },
    }
  };
  </script>
  
  <style scoped>
  .register-container {
    max-width: 400px;
    margin: 2rem auto;
    padding: 1.5rem;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  }
  
  .error-message {
    color: red;
    margin-bottom: 1rem;
  }
  
  form div {
    margin-bottom: 1rem;
  }
  
  button {
    padding: 0.5rem 1rem;
    background-color: var(--mainblue);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  button:hover {
    background-color: darkblue;
  }
  
  p {
    text-align: center;
  }
  
  p a {
    color: var(--mainblue);
    text-decoration: none;
  }
  
  p a:hover {
    text-decoration: underline;
  }
  </style>
  