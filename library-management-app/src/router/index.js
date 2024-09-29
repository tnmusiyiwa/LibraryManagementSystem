import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Home from '../views/HomeView.vue'
import Login from '../views/LoginView.vue'
import Callback from '../views/CallbackView.vue'
import Books from '../views/BooksView.vue'
import BookDetails from '../views/BookDetailsView.vue'
import Dashboard from '../views/DashboardView.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/login', component: Login },
  { path: '/callback', component: Callback },
  { path: '/books', component: Books },
  { path: '/books/:id', component: BookDetails },
  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  await authStore.checkAuth()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else {
    next()
  }
})

export default router
