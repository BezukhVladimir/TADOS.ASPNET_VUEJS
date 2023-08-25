<template>
	<div class="video">
		<div>
			<div><strong>Author:</strong> {{ video.authorEmail }}</div>
			<div><strong>Name:</strong> {{ video.name }}</div>
			<div><strong>URL:</strong> {{ video.url }}</div>
			<iframe width="250" height="141" :src="videoUrl" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" allowfullscreen></iframe>
		</div>

		<my-dialog v-model:show="dialogRateVideoVisible">
			<video-rating-form :users="filteredUsers"
												 :video="video"
												 @rate="rateVideo" />
		</my-dialog>

		<div class="video_buttons">
			<my-button @click="$emit('delete', video)">
				Delete
			</my-button>

			<my-button @click="showDialogRateVideo"
								 style="margin-top: 10px">
				Rate
			</my-button>
		</div>
	</div>
</template>

<script>
import VideoRatingForm from "@/components/Videos/VideoRatingForm";

export default {
	components: {
		VideoRatingForm
	},
	props: {
		video: {
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
			videoUrl: '',
			dialogRateVideoVisible: false,
			ratings: []
		}
	},
	methods: {
		getVideoUrl() {
			this.videoUrl = this.video.url;
		},
		showDialogRateVideo() {
			this.dialogRateVideoVisible = true
		},
		async rateVideo(rating) {
			this.dialogRateVideoVisible = false;
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
		this.getVideoUrl();
	},
	computed: {
		filteredUsers() {
			return this.users.filter(user => user.userId != this.video.authorId);
		}
	}
}
</script>

<style scoped>
	.video {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.video_buttons {
		max-width: 80px;
	}
</style>