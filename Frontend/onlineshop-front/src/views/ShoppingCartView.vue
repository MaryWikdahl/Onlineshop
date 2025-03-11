<template>
  <div class="cart-container">
    <h1 class="cart-title">Varukorg</h1>
    <div v-for="item in cartItems" :key="`${item.id}-${item.selectedSize}`" class="cart-item">
      <button class="heart-button" @click="addToWishlistHandler(item)">
        <i class="fa fa-heart-o"></i>
      </button>
      <router-link :to="'/product/' + item.id">
        <div class="cart-item-image">
          <img :src="item.encodedImage" alt="Produktbild" class="product-image" />
        </div>
      </router-link>
      <div class="cart-item-details">
        <h2 class="cart-item-name">{{ item.name }}</h2>
     
        <p class="cart-item-price">{{ item.price }} SEK</p>
        <p class="cart-item-size">Storlek: {{ item.selectedSize }}</p> <!-- Nytt -->
        <div class="quantity-controls">
          <!-- Minska kvantitet-knappen -->
          <button 
            @click="() => item.quantity -= 1" 
            class="quantity-btn" 
            :disabled="item.quantity <= 1">
            -
          </button>

          <!-- Kvantitet -->
          <input
            type="number"
            v-model.number="item.quantity"
            @change="updateQuantity(item.id, item.selectedSize, item.quantity)"
            class="quantity-input"
            :min="1"
          />

          <!-- Öka kvantitet-knappen -->
          <button 
            @click="() => item.quantity += 1" 
            :disabled="item.quantity >= item.stockQuantity"
            class="quantity-btn">
            +
          </button>
        </div>
        <span v-if="item.stockQuantity < 5 && item.stockQuantity > 0">
          Varning: Denna produkt håller på att ta slut.
        </span>
        <span style="color:red" v-else-if="item.stockQuantity <= 0">
          Denna produkt är slut i lager
        </span>
      </div>
      <button @click="removeFromCart(item.id, item.selectedSize)" class="remove-button">Ta bort</button>
    </div>
    <div class="cart-footer">
      <button @click="clearCart" class="clear-cart-button">Töm varukorgen</button>
      <button class="clear-cart-button" @click="changeOrder(cartItems)">Lägg en order</button>
      <div class="total-price">
        <p>Total: <strong>{{ cartTotal }} SEK</strong></p>
      </div>
    </div>
  </div>
</template>


  
  <script>
  export default {
    computed: {
      cartItems() {
        return this.$store.getters.cartItems;
      },
      cartTotal() {
        return this.$store.getters.cartTotal;
      },
    },
    methods: {
      addToWishlistHandler(productId) {
      alert('Produkten är nu tillagd till din wishlist')
      this.$store.dispatch('addToWishlist', productId);
  
      this. removeFromCart(productId)
    },

 
async changeOrder(cartItems) {
  const token = this.$store.state.token;
  if (!token) {
    alert('Du måste vara inloggad för att lägga en order.');
    return; // Avbryt om användaren inte är inloggad
  }
  try {
    // Visa en bekräftelse-dialog
    const confirmOrder = window.confirm('Vill du verkligen lägga en order?');
    if (!confirmOrder) {
      // Om användaren avbryter, gör inget
      return;
    }

    // Kontrollera lagersaldo för varje produkt i kundvagnen
    for (const item of cartItems) {
     
      const response = await fetch(`https://localhost:7131/api/products/${item.id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error(`Failed to fetch product data for ID: ${item.id}`);
      }

      const productData = await response.json();

  
      // Kontrollera om beställt antal överstiger lagersaldo
      if (item.quantity > productData.stockQuantity) {
        alert(
          `Produkten "${productData.name}" har endast ${productData.stockQuantity} i lager, men du försökte beställa ${item.quantity} stycken.`
        );
        return; // Avbryt om lagret inte räcker
      }
    }

 
    // Bygg order-payload med totalAmount och orderItems
    const orderPayload = {
          totalAmount: this.cartTotal,
          orderItems: cartItems.map((item) => ({
            productId: item.id,
            selectedSize: item.selectedSize,
            quantity: item.quantity,
          })),
    };

    // Skicka POST-förfrågan till backend
    const response = await fetch('https://localhost:7131/api/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json', // Anger att vi skickar JSON
        'Authorization': `Bearer ${token}` 
      },
      body: JSON.stringify(orderPayload), // Omvandla payloaden till JSON-format
    });

    // Kontrollera om förfrågan lyckades
    if (!response.ok) {
      throw new Error('Failed to create order');
    }

    // Läs svaret från backend
    const data = await response.json();
    console.log('Order created successfully:', data);

    // Visa en framgångsmeddelande
    alert('Order skapad framgångsrikt!');

    // Rensa varukorgen om ordern skapades framgångsrikt
    this.clearCart();
  } catch (error) {
    console.error('Error creating order:', error);
    alert('Något gick fel vid skapandet av ordern.');
  }
}
,
      // Uppdatera kvantiteten i Vuex-storen
      updateQuantity(productId, selectedSize, quantity) {
    this.$store.dispatch("updateQuantity", { productId, selectedSize, quantity });
  },

  
    removeFromCart(productId, selectedSize) {
      this.$store.dispatch("removeFromCart", { productId, selectedSize });
    },
    clearCart() {
  this.$store.dispatch('clearCart');
}
    },
  };
  </script>
  
  <style scoped>
     input::-webkit-outer-spin-button,
      input::-webkit-inner-spin-button {
         -webkit-appearance: none;
      }
  .cart-container {
    width: 80%;
    margin: 0 auto;
    padding: 20px;
    font-family: Arial, sans-serif;
  }
  
  .cart-title {
    text-align: center;
    color: #333;
    font-size: 2rem;
    margin-bottom: 20px;
  }
  
  .cart-item {
    display: flex;
    align-items: center;
    justify-content: space-between;
    border-bottom: 1px solid #ccc;
    padding: 15px 0;
    margin-bottom: 10px;
  }
  
  .cart-item-image {
    flex-shrink: 0;
  }
  
  .product-image {
    width: 60px;
    height: 60px;
    object-fit: cover;
    border-radius: 5px;
  }
  
  .cart-item-details {
    flex-grow: 1;
    margin-left: 20px;
  }
  
  .cart-item-name {
    font-size: 1.2rem;
    margin: 0;
    color: #333;
  }
  
  .cart-item-price {
    color: #888;
    margin: 5px 0;
  }
  
  .cart-item-quantity {
    font-size: 0.9rem;
    color: #666;
  }
  .quantity-btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}
  .remove-button {
    padding: 8px 12px;
    background-color: #e53770;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 0.9rem;
  }
  
  .remove-button:hover {
    background-color: #db0049;
  }
  
  .cart-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 30px;
  }
  
  .clear-cart-button {
    padding: 10px 20px;
    background-color: #e53770;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 1rem;
  }
  
  .clear-cart-button:hover {
    background-color: #db0049;
  }
  
  .total-price {
    font-size: 1.2rem;
    font-weight: bold;
  }
  
  /* Stylar för kvantitetskontroller */
  .quantity-controls {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  
  .quantity-btn {
    padding: 5px 10px;
    background-color: #e53770;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  .quantity-btn:hover {
    background-color: #db0049;
  }
  
  .quantity-input {
    width: 40px;
    text-align: center;
    padding: 5px;
    border-radius: 5px;
    border: 1px solid #ccc;
  }

  </style>
  