<template>
  <div class="bg-white shadow-md rounded-lg p-4 mb-4">
    <h3 class="text-xl font-semibold mb-4">{{ book ? 'Update Book' : 'Add New Book' }}</h3>
    <form @submit.prevent="submitForm">
      <div class="mb-4">
        <label for="title" class="block text-gray-700 text-sm font-bold mb-2">Title</label>
        <input
          v-model="formData.title"
          type="text"
          id="title"
          required
          class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        />
      </div>
      <div class="mb-4">
        <label for="author" class="block text-gray-700 text-sm font-bold mb-2">Author</label>
        <input
          v-model="formData.author"
          type="text"
          id="author"
          required
          class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        />
      </div>
      <div class="mb-4">
        <label for="year" class="block text-gray-700 text-sm font-bold mb-2">Year</label>
        <input
          v-model="formData.publicationYear"
          type="number"
          id="year"
          required
          class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        />
      </div>
      <div class="mb-4">
        <label for="category" class="block text-gray-700 text-sm font-bold mb-2">Category</label>
        <input
          v-model="formData.category"
          type="text"
          id="category"
          required
          class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
        />
      </div>
      <div class="flex justify-end">
        <button
          type="submit"
          class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded mr-2"
        >
          {{ book ? 'Update' : 'Add' }}
        </button>
        <button
          type="button"
          @click="cancel"
          class="bg-gray-500 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded"
        >
          Cancel
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const props = defineProps({
  book: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['submit', 'cancel'])

const formData = ref({
  title: '',
  author: '',
  year: null,
  category: ''
})

onMounted(() => {
  if (props.book) {
    formData.value = { ...props.book }
  }
})

const submitForm = () => {
  emit('submit', formData.value)
}

const cancel = () => {
  emit('cancel')
}
</script>
