<template>
  <div v-if="videos.length > 0">
    <h3>List of videos</h3>
    <video-item v-for="video in videos"
               :video="video"
               :users="users"
               :key="video.id"
               @delete="$emit('delete', video)" />
  </div>
  <h2 v-else style="color: red">
    List of videos is empty
  </h2>
</template>

<script>
  import VideoItem from "@/components/Videos/VideoItem";

  export default {
    components: {
      VideoItem
    },
    props: {
      videos: {
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