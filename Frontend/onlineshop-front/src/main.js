import { createApp } from 'vue'
import App from './App.vue'
import './style/main.css'
import router from './router'
import store from './store'
import 'font-awesome/css/font-awesome.css';
createApp(App).use(store).use(router).mount('#app')
