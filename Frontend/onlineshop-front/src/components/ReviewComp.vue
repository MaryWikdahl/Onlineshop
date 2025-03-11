<template>
    <div class="review-container">
      <h1 class="text-2xl font-bold mb-6 text-center">Customer Reviews</h1>
  
      <div >
        <div v-if="reviews.length === 0" class="no-reviews text-gray-500 text-center">
          No reviews available for this product yet. 
        </div>
        <div v-if="averageRating >= 0" class="average-rating text-xl mb-4 text-center">
      Average Rating: {{ averageRating.toFixed(1) }} / 5
    </div>
  
        <div v-for="review in reviews" :key="review.id" class="review-card bg-gray-50 border rounded-lg p-4 mb-4 shadow-md">
          <div class="review-header flex items-center justify-between mb-2">
            <span class="review-user font-semibold text-black">{{ review.userName }}  </span>
            <div class="review-rating flex items-center">
              
              <i v-for="n in 5"  :key="n" :class="n <= review.rating ? 'fa fa-star' : 'fa fa-star-o'"></i>
            </div>
          </div>
          <p class="review-comment text-gray-800">{{ review.comment }}</p>
          <p class="review-date text-gray-500 text-sm mt-2">{{ formatDate(review.createdAt) }}</p>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: "ReviewComp",
    data() {
      return {
        localReviews: [],// Recensioner
        isLoading: true, // Laddar indikator
        errorMessage: "", // Felmeddelande
      };
    }, 
props: {
    idProp:{
        type: Number,
        required:false
    },
    reviews:{
        type: Array,
        required: true
    }
  },
    computed: {
    // Beräknar medelbetyget för alla recensioner
    averageRating() {
  if (this.reviews.length === 0) return -1; // Om inga recensioner finns, returnera -1 (för att inte visa medelbetyget)
  
  const totalRating = this.reviews.reduce((sum, review) => sum + review.rating, 0);
  const avgRating = totalRating / this.reviews.length;
  
  // Skicka beräknat betyg till föräldern

  
  return avgRating; // Returnera medelbetyget för att visa det i komponenten
    }
},
    methods: {
        formatDate(dateString) {
        // Försök att skapa ett Date-objekt från strängen
        const date = new Date(dateString);

        // Kolla om det är ett ogiltigt datum
        if (isNaN(date)) {
            console.error('Ogiltigt datum:', dateString);
            return '';
        }
        // Formatera till ett enklare format (t.ex. YYYY-MM-DD)
        return date.toLocaleDateString(); // Standardformat beroende på systeminställningar
        }




    },
  mounted(){
    console.log(this.reviews)
  }
  
  };
  </script>
  
  <style scoped>
  .review-container {
    max-width: 700px;
    margin: 0 auto;
    padding: 1.5rem;
  }
  
  .loading-spinner {
    text-align: center;
    font-size: 1.2rem;
    color: #4A5568; /* Gray-700 */
  }
  
  .error-message {
    text-align: center;
    font-size: 1rem;
    font-weight: bold;
  }
  
  .no-reviews {
    text-align: center;
    font-size: 1rem;
    margin-top: 1rem;
  }
  
  .review-card {
    background-color: #F9FAFB;
    border: 1px solid #E5E7EB; /* Gray-300 */
    border-radius: 0.75rem;
    padding: 1rem;
    transition: box-shadow 0.3s ease;
  }
  
  .review-card:hover {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }
  
  .review-header {
    font-size: 1rem;
    color: #1F2937; /* Gray-800 */
  }
  
  .review-user {
    color: #1F2937; /* Gray-800 */
  }
  
  .review-rating .star {
    margin-right: 2px;
    font-size: 1.25rem;
  }
  
  .review-comment {
    font-size: 0.95rem;
    line-height: 1.4rem;
    margin-top: 0.5rem;
    color: #374151; /* Gray-700 */
  }
  
  .review-date {
    font-size: 0.85rem;
    color: #6B7280; /* Gray-500 */
  }
  .average-rating {
  font-weight: bold;
  color: #1F2937;
}
  </style>
  