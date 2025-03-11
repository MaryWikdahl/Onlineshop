<template>
    <div class="login-container">
      <h1>Login</h1>
  
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  
      <!-- Inloggningsformulär -->
      <form @submit.prevent="handleLogin" class="LoginForm">
        <div class="TextForm">
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
      </div>
        <button type="submit" :disabled="loading">
          {{ loading ? 'Logging in...' : 'Log In' }}
        </button>
      </form>
  
      <!-- Registreringslänk -->
      <p>Don't have an account? <router-link to="/register">Register here</router-link></p>
    </div>
  </template>
  
  <script>
  import { mapActions, mapGetters } from 'vuex';
  
  export default {
    name: 'LoginView',
    data() {
      return {
        credentials: {
          email: '',
          password: '',
        },
        errorMessage: '',  // Här ska du ta emot felmeddelanden från Vuex
        loading: false, // För att hantera laddningsstatus
      };
    },
    computed: {
      ...mapGetters(['isLoggedIn', 'errorMessage']),  // Lägg till errorMessage från getters
    },
   created() {
    // console.log('är vi inloggad??', this.isLoggedIn)
    //     if(this.isLoggedIn)
    //         this.$router.push('/profile')
   },
    methods: { 
    
      ...mapActions(['loginUser']),
      async handleLogin() {
        this.errorMessage = '';
        this.loading = true;
  
        try {
          await this.loginUser(this.credentials); // Kör Vuex-action
          this.$router.push('/'); // Navigera till hemsidan vid lyckad inloggning
        } catch (error) {
          this.errorMessage = error.message || 'An error occurred during login.';
        } finally {
          this.loading = false;
        }
      },
    },
  };
  </script>
  
  <style scoped>
  .login-container {
    max-width: 400px;
    margin: 2rem auto;
    padding: 1.5rem;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  }
  
  .LoginForm{
    padding: 3rem;
  }
  .TextForm{
    align-items: center;
    display: table-caption;
    text-align: left;
    margin-bottom: 3rem;
    margin-left: 4rem;
  }
  
  .error-message {
    color: red;
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
  </style>
  