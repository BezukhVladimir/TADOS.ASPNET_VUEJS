<template>
	<div class="gallery-image">
		<div>
			<div><strong>Author:</strong> {{ image.authorEmail }}</div>
			<div><strong>Name:</strong> {{ image.name }}</div>
			<div><strong>URL:</strong> {{ image.url }}</div>
			<img :src="imageUrl" width="100" height="100" />
		</div>
	</div>
</template>

<script>
export default {
	props: {
		image: {
			type: Object,
      required: true
		}
	},
	data() {
		return {
			imageUrl: ''
		}
	},
	methods: {
		getImageUrl() {
			this.imageUrl = this.image.url;
		}
	},
	mounted() {
		this.getImageUrl();
	}
}
</script>

<style scoped>
	.gallery-image {
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}
</style>