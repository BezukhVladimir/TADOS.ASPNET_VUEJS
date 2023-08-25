<template>
	<div class="image">
		<div>
			<div><strong>Author:</strong> {{ image.authorEmail }}</div>
			<div><strong>Name:</strong> {{ image.name }}</div>
			<div><strong>URL:</strong> {{ image.url }}</div>
			<img :src="imageUrl" width="100" height="100" />

			<my-dialog v-model:show="dialogRateImageVisible">
				<image-rating-form :users="filteredUsers"
													 :image="image"
										       @rate="rateImage" />
			</my-dialog>

		</div>
		<div class="image_buttons">
			<my-button @click="$emit('delete', image)">
				Delete
			</my-button>

			<my-button @click="showDialogRateImage"
								 style="margin-top: 10px">
				Rate
			</my-button>
		</div>
	</div>
</template>

<script>
import ImageRatingForm from "@/components/Images/ImageRatingForm";

export default {
	components: {
		ImageRatingForm
	},
	props: {
		image: {
			type: Object,
      required: true
		},
		users: {
			type: Array,
			required: true
		}
	},
	data() {
		return {
			imageUrl: '',
			dialogRateImageVisible: false,
			ratings: []
		}
	},
	methods: {
		getImageUrl() {
			this.imageUrl = this.image.url;
		},
		showDialogRateImage() {
			this.dialogRateImageVisible = true
		},
		async rateImage(rating) {
			this.dialogRateImageVisible = false;
			const res = await fetch('https://localhost:44364/api/content/rating/add', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
					raterId: rating.raterUserId,
					ratedId: rating.ratedContentId,
					rate:    rating.rate 
				})
			})
			.then(response => response.json())

			rating.id = res.id;
			this.ratings.push(rating);
		}
	},
	mounted() {
		this.getImageUrl();
	},
	computed: {
		filteredUsers() {
			return this.users
				.filter(user => user.userId != this.image.authorId)
		},
	},
}
</script>

<style scoped>
	.image {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.image_buttons {
		max-width: 80px;
	}
</style>