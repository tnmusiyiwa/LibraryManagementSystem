<template>
  <div class="admin-users">
    <h1 class="text-3xl font-bold mb-6">User Management</h1>
    <button @click="showCreateUserModal = true" class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded mb-4">
      Create User
    </button>
    <table class="min-w-full bg-white">
      <thead>
        <tr>
          <th class="py-2 px-4 border-b">Name</th>
          <th class="py-2 px-4 border-b">Email</th>
          <th class="py-2 px-4 border-b">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="user in users" :key="user.id">
          <td class="py-2 px-4 border-b">{{ user.name }}</td>
          <td class="py-2 px-4 border-b">{{ user.email }}</td>
          <td class="py-2 px-4 border-b">
            <button @click="editUser(user)" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-1 px-2 rounded mr-2">
              Edit
            </button>
            <button @click="deleteUser(user.id)" class="bg-red-500 hover:bg-red-700 text-white font-bold py-1 px-2 rounded">
              Delete
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Create User Modal -->
    <Modal v-if="showCreateUserModal" @close="showCreateUserModal = false">
      <h2 class="text-2xl font-bold mb-4">Create User</h2>
      <form @submit.prevent="createUser">
        <div class="mb-4">
          <label for="name" class="block mb-2">Name</label>
          <input v-model="newUser.name" id="name" type="text" class="w-full px-3 py-2 border rounded" required>
        </div>
        <div class="mb-4">
          <label for="email" class="block mb-2">Email</label>
          <input v-model="newUser.email" id="email" type="email" class="w-full px-3 py-2 border rounded" required>
        </div>
        <div class="mb-4">
          <label for="password" class="block mb-2">Password</label>
          <input v-model="newUser.password" id="password" type="password" class="w-full px-3 py-2 border rounded" required>
        </div>
        <div class="mb-4">
          <label for="role" class="block mb-2">Role</label>
          <select v-model="newUser.role" id="role" class="w-full px-3 py-2 border rounded" required>
            <option value="User">User</option>
            <option value="Admin">Admin</option>
          </select>
        </div>
        <button type="submit" class="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded">
          Create User
        </button>
      </form>
    </Modal>

    <!-- Edit User Modal -->
    <Modal v-if="showEditUserModal" @close="showEditUserModal = false">
      <h2 class="text-2xl font-bold mb-4">Edit User</h2>
      <form @submit.prevent="updateUser">
        <div class="mb-4">
          <label for="edit-name" class="block mb-2">Name</label>
          <input v-model="editingUser.name" id="edit-name" type="text" class="w-full px-3 py-2 border rounded" required>
        </div>
        <div class="mb-4">
          <label for="edit-email" class="block mb-2">Email</label>
          <input v-model="editingUser.email" id="edit-email" type="email" class="w-full px-3 py-2 border rounded" required>
        </div>
        <div class="mb-4">
          <label for="edit-role" class="block mb-2">Role</label>
          <select v-model="editingUser.role" id="edit-role" class="w-full px-3 py-2 border rounded" required>
            <option value="User">User</option>
            <option value="Admin">Admin</option>
          </select>
        </div>
        <button type="submit" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          Update User
        </button>
      </form>
    </Modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useNotification } from '../composables/useNotification'
import api from '../services/api'
import Modal from '../components/ModalComponent.vue'

const { showNotification } = useNotification()

const users = ref([])
const showCreateUserModal = ref(false)
const showEditUserModal = ref(false)
const newUser = ref({ name: '', email: '', password: '', role: 'User' })
const editingUser = ref({ id: '', name: '', email: '', role: '' })

const fetchUsers = async () => {
  try {
    const response = await api.get('/users')
    users.value = response.data
  } catch (error) {
    console.error('Failed to fetch users:', error)
    showNotification('Failed to fetch users', 'error')
  }
}

const createUser = async () => {
  try {
    await api.post('/users', newUser.value)
    showNotification('User created successfully', 'success')
    showCreateUserModal.value = false
    newUser.value = { name: '', email: '', password: '', role: 'User' }
    await fetchUsers()
  } catch (error) {
    console.error('Failed to create user:', error)
    showNotification('Failed to create user', 'error')
  }
}

const editUser = (user) => {
  editingUser.value = { ...user }
  showEditUserModal.value = true
}

const updateUser = async () => {
  try {
    await api.put(`/users/${editingUser.value.id}`, editingUser.value)
    showNotification('User updated successfully', 'success')
    showEditUserModal.value = false
    await fetchUsers()
  } catch (error) {
    console.error('Failed to update user:', error)
    showNotification('Failed to update user', 'error')
  }
}

const deleteUser = async (userId) => {
  if (confirm('Are you sure you want to delete this user?')) {
    try {
      await api.delete(`/users/${userId}`)
      showNotification('User deleted successfully', 'success')
      await fetchUsers()
    } catch (error) {
      console.error('Failed to delete user:', error)
      showNotification('Failed to delete user', 'error')
    }
  }
}

onMounted(fetchUsers)
</script>