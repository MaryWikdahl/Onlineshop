<template>
    <div class="change-password-container">
      <h1>Ändra Lösenord</h1>
      <form @submit.prevent="handleChangePassword" class="change-password-form">
        <div class="form-group">
          <label for="old-password">Nuvarande Lösenord</label>
          <input
            type="password"
            id="old-password"
            v-model="oldPassword"
            placeholder="Ange ditt nuvarande lösenord"
            required
          />
        </div>
  
        <div class="form-group">
          <label for="new-password">Nytt Lösenord</label>
          <input
            type="password"
            id="new-password"
            v-model="newPassword"
            placeholder="Ange ditt nya lösenord"
            required
          />
        </div>
  
        <div class="form-group">
          <label for="confirm-password">Bekräfta Nytt Lösenord</label>
          <input
            type="password"
            id="confirm-password"
            v-model="confirmPassword"
            placeholder="Bekräfta ditt nya lösenord"
            required
          />
        </div>
  
        <button type="submit" class="btn submit" :disabled="isLoading">
          {{ isLoading ? "Sparar..." : "Ändra Lösenord" }}
        </button>
      </form>
  
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <p v-if="successMessage" class="success-message">{{ successMessage }}</p>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        oldPassword: '',
        newPassword: '',
        confirmPassword: '',
        successMessage: '',
      };
    },
    computed: {
      errorMessage() {
        return this.$store.state.errorMessage; // Hämta felmeddelande från Vuex
      },
      isLoading() {
        return this.$store.state.loading; // Hämta loading-status från Vuex
      },
    },
    methods: {
      async handleChangePassword() {
        if (this.newPassword !== this.confirmPassword) {
          this.successMessage = '';
          alert('Det nya lösenordet matchar inte bekräftelsen.');
          return;
        }
  
        try {
          await this.$store.dispatch('changePassword', {
            oldPassword: this.oldPassword,
            newPassword: this.newPassword,
          });
          this.successMessage = 'Lösenordet har ändrats framgångsrikt!';
          this.oldPassword = '';
          this.newPassword = '';
          this.confirmPassword = '';
        } catch (error) {
          console.error('Error changing password:', error);
          this.successMessage = '';
        }
      },
    },
  };
  </script>
  
  <style scoped>
  .change-password-container {
    max-width: 500px;
    margin: 2rem auto;
    padding: 2rem;
    border: 1px solid #ddd;
    border-radius: 10px;
    background-color: #fff;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }
  
  h1 {
    text-align: center;
    color: #333;
    margin-bottom: 1.5rem;
  }
  
  .form-group {
    margin-bottom: 1rem;
  }
  
  label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: bold;
    color: #555;
  }
  
  input {
    width: 100%;
    padding: 0.8rem;
    font-size: 1rem;
    border: 1px solid #ccc;
    border-radius: 5px;
    box-sizing: border-box;
  }
  
  input:focus {
    border-color: #3498db;
    outline: none;
    box-shadow: 0 0 5px rgba(52, 152, 219, 0.5);
  }
  
  button {
    width: 100%;
    padding: 0.8rem;
    font-size: 1rem;
    font-weight: bold;
    background-color: #3498db;
    color: #fff;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    text-transform: uppercase;
  }
  
  button:disabled {
    background-color: #95a5a6;
    cursor: not-allowed;
  }
  
  button:hover:not(:disabled) {
    background-color: #2980b9;
  }
  
  .error-message {
    color: red;
    font-weight: bold;
    text-align: center;
    margin-top: 1rem;
  }
  
  .success-message {
    color: green;
    font-weight: bold;
    text-align: center;
    margin-top: 1rem;
  }
  </style>
  