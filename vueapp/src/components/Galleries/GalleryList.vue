<template>
  <div v-if="galleries.length > 0">
    <h3>List of galleries</h3>
    <gallery-item v-for="gallery in galleries"
                  :gallery="gallery"
                  :users="users"
                  :key="gallery.id"
                  @delete="$emit('delete', gallery)" />
  </div>
  <h2 v-else style="color: red">
    List of galleries is empty
  </h2>
</template>

<script>
  import GalleryItem from "@/components/Galleries/GalleryItem";

  export default {
    components: {
      GalleryItem
    },
    props: {
      galleries: {
        type: Array,
        required: true
      },
      users: {
        type: Array,
        required: true
      }
    }
  }
</script>

<style scoped>
	* {
	  margin-top: 15px;
	}
</style>