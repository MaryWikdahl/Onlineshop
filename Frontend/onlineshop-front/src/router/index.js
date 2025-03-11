import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import AdminPView from '../views/Admin/AdminProduktView.vue'; 
import AdminCView from '../views/Admin/AdminCategoryView.vue'; 
import AdminUserView from '../views/Admin/AdminUserView.vue'; 
import AdminOrderView from '../views/Admin/AdminOrderView.vue';
import ProfileView from '../views/ProfileView.vue';
import edituser from '../views/EditUserView.vue';
import { useStore } from 'vuex'; // Import useStore from Vuex

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/about',
    name: 'about',
   
    component: () => import(/* webpackChunkName: "about" */ '../views/AboutView.vue')
  },
  {
    path: '/products',
    name: 'products',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/ProductsView.vue')
  },
  {
    path: '/product/:id',
    name: 'product',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/SingelProductView.vue')
  },
  {
    path: '/category/:id',
    name: 'Category',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/CategoriesView.vue')
  },
  {
    path: '/cart',
    name: 'cart',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/ShoppingCartView.vue')
  },
  {
    path: '/orders',
    name: 'orders',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/OrdersView.vue')
  },
  {
    path: '/SubCatView/:subCategoryId',
    name: 'SubCatView',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/SubCatView.vue'),
    props: true,
  },
  {
    path: '/login',
    name: 'login',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/LoginView.vue'),
    props: true,
  },
  {
    path: '/wishlist',
    name: 'wishlist',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/WishlistView.vue'),
    props: true,
  },
  {
    path: '/reviewitem/:id',
    name: 'reviewitem',
    component: () => import(/* webpackChunkName: "about" */ '../views/ReviewItemView.vue'),
  
    props: true, // Tillåter att skicka :id som prop till komponenten
  },
  {
    path: '/profile',
    name: 'profile',
 
    component: ProfileView,
    beforeEnter: (to, from, next) => {
      const store = useStore(); // Om du använder Vuex som global
      if (store.getters.isLoggedIn) {
        next();
      } else {
        next('/home'); // Skicka användaren till en "otillåten"-vy
      }
    },
  },
  {
    path: '/edituser',
    name: 'edituser',
 
    component: edituser,
    beforeEnter: (to, from, next) => {
      const store = useStore(); // Om du använder Vuex som global
      if (store.getters.isLoggedIn) {
        next();
      } else {
        next('/home'); // Skicka användaren till en "otillåten"-vy
      }
    },
  },
  {
    path: '/login',
    name: 'login',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/LoginView.vue'),
    props: true,
  },
  {
    path: '/register',
    name: 'register',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/RegisterView.vue'),
    props: true,
  },
  {
    path: '/changepassword',
    name: 'rchangepassword',
 
    component: () => import(/* webpackChunkName: "about" */ '../views/ChangePassword.vue'),
    props: true,
  },

    {
      path: '/adminprodukt',
      name: 'AdminProdukt',
      component: AdminPView,
      beforeEnter: (to, from, next) => {
        const store = useStore(); // Om du använder Vuex som global
        if (store.getters.isAdmin) {
          next();
        } else {
          next('/home'); // Skicka användaren till en "otillåten"-vy
        }
      },
    },
    {
      path: '/admincategory',
      name: 'AdminCategory',
      component: AdminCView,
      beforeEnter: (to, from, next) => {
        const store = useStore(); // Om du använder Vuex som global
        if (store.getters.isAdmin) {
          next();
        } else {
          next('/home'); // Skicka användaren till en "otillåten"-vy
        }
      },
    },
    {
      path: '/adminUser',
      name: 'AdminUser',
      component: AdminUserView,
      beforeEnter: (to, from, next) => {
        const store = useStore(); // Om du använder Vuex som global
        if (store.getters.isAdmin) {
          next();
        } else {
          next('/home'); // Skicka användaren till en "otillåten"-vy
        }
      },
    },
    {
      path: '/adminOrders',
      name: 'AdminOrders',
      component: AdminOrderView,
      beforeEnter: (to, from, next) => {
        const store = useStore(); // Om du använder Vuex som global
        if (store.getters.isAdmin) {
          next();
        } else {
          next('/home'); // Skicka användaren till en "otillåten"-vy
        }
      },
    },

  

]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
