<template>
    <div v-if="user" class="profile-container">
      <h2>Redigera profil</h2>
      <form @submit.prevent="updateProfile">
        <label for="name">Namn:</label>
        <input v-model="user.name" type="text" id="name" required />
  
        <label for="address">Adress:</label>
        <input v-model="user.address" type="text" id="address" required />
  
        <label for="mobile">Mobil:</label>
        <input v-model="user.mobile" type="text" id="mobile" required />
  
        <label for="city">Stad:</label>
        <input v-model="user.city" type="text" id="city" required />
  
        <label for="zipcode">Postnummer:</label>
        <input v-model="user.zipcode" type="text" id="zipcode" required />
  
        <button type="submit" class="save-button">Spara ändringar</button>
      </form>
    </div>
    <div v-else class="no-profile">
      <p>Inga användaruppgifter hittades.</p>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        user: {
          name: '',
          address: '',
          mobile: '',
          city: '',
          zipcode: ''
        },
        loading: false,
      };
    },
    computed: {
      // Här kan du definiera en getter för att hämta användardata om det behövs
      token() {
        return this.$store.state.token;
      },
    },
    methods: {
      async fetchUserData() {
        this.loading = true;
        try {
          const response = await fetch("https://localhost:7131/api/users/profile", {
            method: "GET",
            headers: {
              Authorization: `Bearer ${this.token}`,
            },
          });
  
          if (!response.ok) {
            throw new Error("Kunde inte hämta användardata.");
          }
  
          const data = await response.json();
          this.user = { ...data };
        } catch (error) {
          console.error(error.message);
        } finally {
          this.loading = false;
        }
      },
  
      async updateProfile() {
  this.loading = true;
  try {
    const response = await fetch("https://localhost:7131/api/users/profile", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${this.token}`,
      },
      body: JSON.stringify(this.user),
    });

    if (!response.ok) {
      const errorText = await response.text();  // Läs svaret som text om status inte är OK
      throw new Error(errorText);  // Kasta ett fel med det textbaserade svaret
    }

    // Läs texten som svar, även om det är text och inte JSON
    const data = await response.text();  
    console.log("Svar från servern:", data);

    alert(data || "Profilen har uppdaterats!");
  } catch (error) {
    console.error(error.message);
    alert("Ett fel inträffade vid uppdatering av användarens profil.");
  } finally {
    this.loading = false;
  }
}

    },
    mounted() {
      this.fetchUserData(); // Hämta användardata när komponenten laddas
    },
  };
  </script>
  
  <style scoped>
  .profile-container {
    padding: 2rem;
  }
  
  h2 {
    margin-bottom: 1rem;
    color: var(--mainblue);
  }
  
  form {
    display: flex;
    flex-direction: column;
  }
  
  label {
    margin-top: 1rem;
  }
  
  input {
    margin-top: 0.5rem;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 5px;
  }
  
  button.save-button {
    margin-top: 1.5rem;
    padding: 10px;
    background-color: var(--mainblue);
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  button.save-button:hover {
    background-color: darkblue;
  }
  
  .no-profile {
    padding: 2rem;
    text-align: center;
    color: #777;
  }
  </style>
  