<template>
	<div class="gallery">
		<div>
			<div><strong>Author:</strong> {{ gallery.authorEmail }}</div>
			<div><strong>Name:</strong> {{ gallery.name }}</div>
			<div><strong>Cover URL:</strong> {{ gallery.cover.url }}</div>
			<img :src="coverImageUrl" width="100" height="100" />

			<div>
				<strong>Images:</strong>
				<gallery-image-item v-for="image in gallery.images"
														:image="image"
														:key="image.id" />
			</div>

			<my-dialog v-model:show="dialogRateGalleryVisible">
				<gallery-rating-form :users="filteredUsers"
													   :gallery="gallery"
													   @rate="rateGallery" />
			</my-dialog>
		</div>

		<div class="gallery_buttons">
			<my-button @click="$emit('delete', gallery)">
				Delete
			</my-button>

			<my-button @click="showDialogRateGallery"
								 style="margin-top: 10px">
				Rate
			</my-button>
		</div>
	</div>
</template>

<script>
	import GalleryImageItem  from "@/components/Galleries/GalleryImageItem";
	import GalleryRatingForm from "@/components/Galleries/GalleryRatingForm";

	export default {
		components: {
			GalleryImageItem, GalleryRatingForm
		},
		props: {
			gallery: {
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
				coverImageUrl: '',
				dialogRateGalleryVisible: false,
				ratings: []
			}
		},
		methods: {
			getCoverImageUrl() {
				this.coverImageUrl = this.gallery.cover.url;
			},
			showDialogRateGallery() {
				this.dialogRateGalleryVisible = true
			},
			async rateGallery(rating) {
				this.dialogRateGalleryVisible = false;
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
			this.getCoverImageUrl();
		},
		computed: {
			filteredUsers() {
				return this.users
					.filter(user => user.userId != this.gallery.authorId)
			}
		}
	}
</script>

<style scoped>
	.gallery {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.gallery_buttons {
		max-width: 80px;
	}
</style>