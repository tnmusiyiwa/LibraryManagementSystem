import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import Home from '../views/HomeView.vue'
import Login from '../views/LoginView.vue'
import Dashboard from '../views/DashboardView.vue'
import AdminDashboard from '../views/AdminDashboard.vue'
import AdminUsers from '../views/AdminUsers.vue'
import AdminBooks from '../views/AdminBooks.vue'
import AdminNotifications from '../views/AdminNotifications.vue'
import BooksView from '@/views/BooksView.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/login', component: Login },
  { path: '/books', component: BooksView },
  {
    path: '/dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  },
  {
    path: '/admin',
    component: AdminDashboard,
    meta: { requiresAuth: true, requiresAdmin: true },
    children: [
      { path: '', redirect: { name: 'AdminUsers' } },
      { path: 'users', name: 'AdminUsers', component: AdminUsers },
      { path: 'books', name: 'AdminBooks', component: AdminBooks },
      { path: 'notifications', name: 'AdminNotifications', component: AdminNotifications }
    ]
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
  } else if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router
