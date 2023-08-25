<template>
  <div v-if="images.length > 0">
    <h3>List of images</h3>
    <image-item v-for="image in images"
                :image="image"
                :users="users"
                :key="image.id"
                @delete="$emit('delete', image)" />
  </div>
  <h2 v-else style="color: red">
    List of images is empty
  </h2>
</template>

<script>
  import ImageItem from "@/components/Images/ImageItem";

  export default {
    components: {
      ImageItem
    },
    props: {
      images: {
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