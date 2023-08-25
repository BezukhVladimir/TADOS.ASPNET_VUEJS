<template>
  <div v-if="contents.length > 0">
    <h3>List of contents</h3>
    <content-item v-for="content in contents"
                :content="content"
                :key="content.id"/>
  </div>
  <h2 v-else style="color: red">
    List of contents is empty
  </h2>
</template>

<script>
  import ContentItem from "@/components/Leaderboards/Contents/ContentItem";

  export default {
    components: {
      ContentItem
    },
    props: {
      contents: {
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