<template>
    <div class="reviewForm">
      <h1 class="form-title">Write a Review for {{ "\"" + product.name + "\"" }}</h1>
 
  
      <!-- Visa felmeddelande om något går fel -->
      <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
  
      <!-- Formulär för recension -->
      <form @submit.prevent="submitReview">
        <div class="form-group">
          <label for="rating">Rating (1-5):</label>
          <input type="number" v-model="rating" min="1" max="5" required />
        </div>
        <div class="form-group">
          <label for="comment">Comment:</label>
          <textarea v-model="comment" required></textarea>
        </div>
        <div class="form-actions">
          <button type="submit">Submit Review</button>
        </div>
      </form>
    </div>
  </template>
  
  <script>
  import { mapState } from 'vuex';
  export default {
    name: 'ReviewForm',
    data() {
      return {
        product: {}, 
        rating: 0,        // Användarens rating
        comment: '',      // Användarens kommentar
        errorMessage: '',  // Felmeddelande
      };
    },
    computed: {
      ...mapState(['token']),
    },
  
    methods: {

        async getProduct() {
      try {
        const response = await fetch(`https://localhost:7131/api/products/${this.$route.params.id}`);
        if (!response.ok) throw new Error('Failed to fetch product');
        const data = await response.json();
        this.product = data; // Sätt produktdata i state
      } catch (error) {
        console.error(error);
        this.errorMessage = 'Kunde inte hämta produkten.'; // Visa felmeddelande
      }
    },
      // Skicka recension till API
      async submitReview() {
        const review = {
          productId: this.$route.params.id,  // Produkt-ID från route
          rating: this.rating,
          comment: this.comment
        };
  
        try {
          const response = await fetch(`https://localhost:7131/api/Reviews/product/${this.$route.params.id}`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${this.token}`,
            },
            body: JSON.stringify(review),
          });
          
          if (response.ok) {
            alert('Review submitted successfully!');
            // Möjliggör att kanske navigera bort eller uppdatera vyn här
          }
          else if (response.status === 409){
            alert('Du har redan lämnat en raiting på denna produkt')
          }
           else {
            alert('Failed to submit review');
          }
        } catch (error) {
          console.error("Error submitting review:", error);
          this.errorMessage = "There was an error submitting your review.";
        }
      },
    },
    created(){
        this.getProduct();
    }
  };
  </script>
  
  <style scoped>
  /* Grundläggande styling för form */
  .reviewForm {
    max-width: 500px;
    margin: 0 auto;
    padding: 20px;
    background-color: #f9f9f9;
    border-radius: 8px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }
  
  .form-title {
    text-align: center;
    font-size: 1.5rem;
    color: #333;
    margin-bottom: 20px;
  }
  
  /* Felmeddelande stil */
  .error-message {
    color: red;
    margin-bottom: 1rem;
    text-align: center;
  }
  
  /* Formulärlayout */
  form {
    display: flex;
    flex-direction: column;
  }
  
  /* Gruppindelning för varje input */
  .form-group {
    margin-bottom: 20px;
  }
  
  /* Label-stil */
  label {
    display: block;
    font-weight: bold;
    margin-bottom: 5px;
    font-size: 1rem;
    color: #555;
  }
  
  /* Input och textarea */
  input,
  textarea {
    width: 100%;
    padding: 10px;
    margin-top: 5px;
    border-radius: 5px;
    border: 1px solid #ddd;
    font-size: 1rem;
  }
  
  textarea {
    resize: vertical;
    min-height: 100px;
  }
  
  /* Knappstil */
  button {
    background-color: var(--mainblue);
    color: white;
    border: none;
    padding: 12px 20px;
    cursor: pointer;
    border-radius: 5px;
    font-size: 1.1rem;
    transition: background-color 0.3s ease;
    width: 100%;
    text-transform: uppercase;
    font-weight: bold;
  }
  
  button:hover {
    background-color: #0056b3;
  }
  
  /* Justera knappens hover-effekt */
  button:active {
    background-color: #00408a;
  }
  
  /* Stil för formens actions */
  .form-actions {
    text-align: center;
  }
  </style>
  