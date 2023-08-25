<template>
	<div class="article">
		<div>
			<div><strong>Author:</strong> {{ article.authorEmail }}</div>
			<div><strong>Name:</strong> {{ article.name }}</div>
			<div><strong>Text:</strong> {{ article.text }}</div>
		</div>

		<my-dialog v-model:show="dialogRateArticleVisible">
			<article-rating-form :users="filteredUsers"
												   :article="article"
												   @rate="rateArticle" />
		</my-dialog>

		<div class="article_buttons">
			<my-button @click="$emit('delete', article)">
				Delete
			</my-button>

			<my-button @click="showDialogRateArticle"
								 style="margin-top: 10px">
				Rate
			</my-button>
		</div>
	</div>
</template>

<script>
import ArticleRatingForm from "@/components/Articles/ArticleRatingForm";

export default {
	components: {
		ArticleRatingForm
	},
	props: {
		article: {
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
			articleText: '',
			dialogRateArticleVisible: false,
			ratings: []
		}
	},
	methods: {
		getArticleText() {
			this.articleText = this.article.text;
		},
		showDialogRateArticle() {
			this.dialogRateArticleVisible = true
		},
		async rateArticle(rating) {
			this.dialogRateArticleVisible = false;
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
		this.getArticleText();
	},
	computed: {
		filteredUsers() {
			return this.users
				.filter(user => user.userId != this.article.authorId)
		},
	}
}
</script>

<style scoped>
	.article {
		padding: 15px;
		border: 2px solid dodgerblue;
		max-width: 490px;
		margin-top: 15px;
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.article_buttons {
		max-width: 80px;
	}
</style>