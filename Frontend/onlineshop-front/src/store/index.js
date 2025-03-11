import { saveToken, getToken, removeToken, saveUser, getUser, removeUser, saveCart, getCart, clearCart } from './localStorage';
import { createStore } from 'vuex';
// Importera router i din store-fil
import router from '@/router';

export default createStore({
  state: {
    cart: getCart(), // Hämta kundvagnen från localStorage vid start
    user: getUser(), // Hämta användare från localStorage vid start
    token: getToken() || '', // Hämta token från localStorage vid start
    errorMessage: '',
    loading: false,
    isAuthenticated: !!getToken(), // Kontrollera om vi har en token vid start
    isAdmin: getUser() ? getUser().isAdmin : false,
    orders: [],
    searchQuery: '',
    isItemClicked: true,
    wishlist: [], // Lägg till wishlist
  },
  mutations: {
    // Lägg till produkt i wishlist
    ADD_TO_WISHLIST(state, product) {
      if (!state.wishlist) state.wishlist = []; // Skapa wishlist om den inte finns
      const existingProduct = state.wishlist.find(item => item.id === product.id);
      if (!existingProduct) {
        state.wishlist.push(product);
      }
    },
    SET_SEARCH_QUERY(state, query) {
        state.searchQuery = query;
      },
      SET_IS_ITEM_CLICKED(state, isClicked) {
        state.isItemClicked = isClicked;
      },
    
    // Rensa errorMessage
    RESET_ERROR_MESSAGE(state) {
      state.errorMessage = '';
    },

    // Logga ut vid ogiltig token
    LOGOUT_INVALID_TOKEN(state) {
      state.user = null;
      state.token = '';
      state.isAuthenticated = false;
      state.orders = [];
      localStorage.removeItem('token');
      removeToken();
      removeUser();
      clearCart(); // Ta bort varukorgen vid utloggning
    },
    
    // Lägg till produkt i varukorgen
    ADD_TO_CART(state, product) {
      const existingProduct = state.cart.find(
        item => item.id === product.id && item.selectedSize === product.selectedSize
      );
    
      if (existingProduct) {
        existingProduct.quantity++; // Öka kvantiteten om produkten redan finns i varukorgen
      } else {
        // Lägg till ny produkt i varukorgen och inkludera stockQuantity
        state.cart.push({
          ...product,
          quantity: 1, // Standardkvantitet är 1
          stockQuantity: product.stockQuantity // Lägg till stockQuantity
        });
      }
    
      saveCart(state.cart); // Spara uppdaterad varukorg
    },

    // Logga ut användaren
    LOGOUT(state) {
      state.user = null;
      state.token = '';
      state.isAuthenticated = false;
      state.orders = [];
      localStorage.removeItem('token');
      removeToken();
      removeUser();
      router.push('/login'); 
    },

    // Sätt felmeddelande
    SET_ERROR_MESSAGE(state, message) {
      state.errorMessage = message;
    },

    // Sätt loading
    SET_LOADING(state, status) {
      state.loading = status;
    },

    // Sätt ordrar
    SET_ORDERS(state, orders) {
      state.orders = orders;
    },

    // Sätt autentisering
    SET_AUTHENTICATED(state, status) {
      state.isAuthenticated = status;
    },

    // Sätt användaren
    SET_USER(state, user) {
      state.user = user;
      state.isAuthenticated = true;
      state.isAdmin = user.isAdmin || false;
      saveUser(user);
    },

    // Sätt token
    SET_TOKEN(state, token) {
      state.token = token;
      localStorage.setItem('token', token);
      saveToken(token);
    },

    // Uppdatera kvantiteten av en produkt i varukorgen
    UPDATE_QUANTITY(state, { productId, selectedSize, quantity }) {
      const product = state.cart.find(item => item.id === productId);
      if (product) {
        const productVariant = product.productInfo.find(info => info.size === selectedSize);
        if (productVariant) {
          productVariant.quantity = quantity; // Uppdatera kvantiteten för den specifika storleken
          saveCart(state.cart); // Spara den uppdaterade varukorgen
        }
      }
    },

    // Töm varukorgen
    CLEAR_CART(state) {
      state.cart = [];
      clearCart();
    },
  },
  actions: {
    async addToWishlist({ commit, state }, product) {
        const token = state.token;
        if (!token) {
          // Visa alert om användaren inte är inloggad
          alert('Du måste vara inloggad för att spara denna produkt till din wishlist');
          return; // Avbryt åtgärden om inte inloggad
        }
    
        try {
          const response = await fetch('https://localhost:7131/api/wishlist', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`, // Skicka token i headern för autentisering
            },
            body: JSON.stringify({ ProductId: product.id }), // Skicka produktens ID
          });
    
          if (!response.ok) {
            const error = await response.json();
            commit('SET_ERROR_MESSAGE', error.message || 'Failed to add to wishlist.');
            return;
          }
    
          // Om produkten lades till, uppdatera Vuex state
          commit('ADD_TO_WISHLIST', product);
        } catch (error) {
          commit('SET_ERROR_MESSAGE', 'An error occurred while adding to wishlist.');
        }
      },
      
    setSearchQuery({ commit }, query) {
        //console.log("Vuex-mutation: Sökfråga satt till:", query);
        commit('SET_SEARCH_QUERY', query);
      },
      setIsItemClicked({ commit }, isClicked) {
        commit('SET_IS_ITEM_CLICKED', isClicked);
      },
  
      updateIsItemClicked({ commit }, status) {
        commit('setIsItemClicked', status); // Action för att uppdatera isItemClicked
      },
      // Lägg till produkt i varukorgen
      addToCart({ commit }, product) {
        commit('ADD_TO_CART', product);
      },
  
      // Ta bort produkt från varukorgen
      removeFromCart({ commit }, productId) {
        commit('REMOVE_FROM_CART', productId);
      },
  
      // Töm varukorgen
      clearCart({ commit }) {
        commit('CLEAR_CART');
      },
         // Uppdatera kvantiteten av en produkt i varukorgen
    updateQuantity({ commit }, payload) {
        commit('UPDATE_QUANTITY', payload);
      },
    // Action för att kontrollera om token är giltig
    async checkTokenValidity({ commit, state }) {
      const token = state.token;
      if (!token) {
        commit('LOGOUT'); // Logga ut om inget token finns
        return;
      }
    
      try {
        const response = await fetch('https://localhost:7131/api/token/validate', {  // API-endpoint för tokenverifiering
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        });
    
        if (!response.ok) {
          commit('LOGOUT_INVALID_TOKEN'); // Logga ut om token inte är giltig
          commit('SET_ERROR_MESSAGE', 'Token is invalid. Please log in again.');
        }
      } catch (error) {
        commit('SET_ERROR_MESSAGE', 'Error verifying token.');
      }
    },
    

    // Starta tokenvalidering var 5:e minut
    startTokenValidation({ dispatch }) {
      setInterval(() => {
        dispatch('checkTokenValidity');
      }, 5 * 60 * 1000); // Kollar var 5:e minut (5 * 60 * 1000 ms)
    },

    // Login action
    async loginUser({ commit, dispatch }, credentials) {
      try {
        commit('SET_ERROR_MESSAGE', '');  // Rensa tidigare felmeddelanden
        const response = await fetch('https://localhost:7131/api/users/login', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(credentials),
        });

        if (response.ok) {
          const data = await response.json();
          commit('SET_TOKEN', data.token);
          commit('SET_AUTHENTICATED', true);
          await dispatch('fetchUser');
          dispatch('resetErrorMessage');  // Rensa errorMessage vid lyckad inloggning
        } else {
          commit('SET_ERROR_MESSAGE', 'Invalid credentials, please try again.');
        }
      } catch (error) {
        commit('SET_ERROR_MESSAGE', 'An error occurred during login. Please try again.');
      }
    },

    // Fetch användardata
    async fetchUser({ commit }) {
      const token = localStorage.getItem('token');
      if (!token) {
        commit('SET_ERROR_MESSAGE', 'You need to be logged in to view your profile.');
        return;
      }
    
      try {
        const response = await fetch('https://localhost:7131/api/users/profile', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
        });
    
        if (!response.ok) {
          // Om responsen inte är OK, kolla om det beror på ogiltig token
          if (response.status === 401) {  // 401 är statuskod för ogiltig token
            commit('LOGOUT_INVALID_TOKEN');  // Logga ut användaren
            commit('SET_ERROR_MESSAGE', 'Your session has expired. Please log in again.');
          } else {
            commit('SET_ERROR_MESSAGE', 'Failed to fetch user profile');
          }
        } else {
          const userData = await response.json();
          commit('SET_USER', userData);
        }
      } catch (error) {
        commit('SET_ERROR_MESSAGE', 'An error occurred while fetching your profile.');
      }
    },
    
    // Logga ut
    logout({ commit, dispatch }) {
      commit('LOGOUT');
      dispatch('resetErrorMessage');  // Rensa errorMessage vid utloggning
    },

    // Rensa error message
    resetErrorMessage({ commit }) {
      commit('RESET_ERROR_MESSAGE');
    },
   // Registrering action
   async register({ commit }, credentials) {
    commit('SET_LOADING', true);
    commit('SET_ERROR_MESSAGE', ''); 

    try {
      const response = await fetch('https://localhost:7131/api/users/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        const errorData = await response.json();
        commit('SET_ERROR_MESSAGE', errorData.message || 'Registration failed');
        throw new Error(errorData.message || 'Registration failed');
      }

      const data = await response.json();
      commit('SET_USER', data);
      commit('SET_TOKEN', data.token);
      return data;
    } catch (error) {
      commit('SET_ERROR_MESSAGE', error.message || 'An error occurred during registration.');
    } finally {
      commit('SET_LOADING', false);
    }
  },
  
  async changePassword({ commit }, { oldPassword, newPassword }) {
    commit('SET_LOADING', true);
    commit('SET_ERROR_MESSAGE', ''); 

    try {
      const token = localStorage.getItem('token');

      const response = await fetch('https://localhost:7131/api/users/change-password', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({ oldPassword, newPassword }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        commit('SET_ERROR_MESSAGE', errorData.message || 'Password change failed');
        throw new Error(errorData.message || 'Password change failed');
      }

      const data = await response.json();
      return data;
    } catch (error) {
      commit('SET_ERROR_MESSAGE', error.message || 'An error occurred while changing password.');
    } finally {
      commit('SET_LOADING', false);
    }
  },
    // Fetch orders
    async fetchOrders({ commit }) {
      const token = localStorage.getItem('token');
      if (!token) {
        commit('SET_ERROR_MESSAGE', 'You need to be logged in to view your orders.');
        return;
      }

      commit('SET_LOADING', true);
      try {
        const response = await fetch('https://localhost:7131/api/users/orders', {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
        });

        if (response.ok) {
          const orders = await response.json();
          commit('SET_ORDERS', orders);
        } else {
          commit('SET_ERROR_MESSAGE', 'Failed to fetch orders.');
        }
      } catch (error) {
        commit('SET_ERROR_MESSAGE', 'An error occurred while fetching orders.');
      } finally {
        commit('SET_LOADING', false);
      }
    },
  },
  getters: {
    searchQuery: state => state.searchQuery,
    isItemClicked: state => state.isItemClicked,

    cartItems(state) {
      return state.cart;
    },

    cartCount(state) {
      return state.cart.reduce((total, product) => total + product.quantity, 0);
    },

    cartTotal(state) {
      return state.cart.reduce((total, product) => total + product.price * product.quantity, 0);
    },

    isLoggedIn(state) {
      return !!state.token;
    },

    user(state) {
      return state.user;
    },

    errorMessage(state) {
      return state.errorMessage;
    },

    isLoading(state) {
      return state.loading;
    },

    isAuthenticated(state) {
      return state.isAuthenticated;
    },

    isAdmin(state) {
      return state.isAdmin;
    },

    orders(state) {
      return state.orders;
    },

    // Lägg till getter för wishlist
    wishlist(state) {
      return state.wishlist;
    },
  },
});
