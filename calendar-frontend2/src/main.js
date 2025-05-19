import { createApp } from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify'  // Mengimpor Vuetify
import VCalendar from 'v-calendar'  // Mengimpor v-calendar
import 'v-calendar/dist/style.css'  // Mengimpor style untuk v-calendar
import router from './router' // Mengimpor Router


const app = createApp(App)

app.use(vuetify)  // Menggunakan Vuetify
app.use(VCalendar)  // Menggunakan v-calendar
app.use(router)
app.mount('#app')

