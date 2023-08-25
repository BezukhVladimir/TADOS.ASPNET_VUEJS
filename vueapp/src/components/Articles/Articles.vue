<template>
	<div>
		<my-input v-model="contentSearch"
							type="text"
							placeholder="Article search"></my-input>

		<my-dialog v-model:show="dialogAddArticleVisible">
			<article-form :users="users"
								 @add="addArticle" />
		</my-dialog>
		<article-list :articles="filteredArticles"
									:users="users"
							    @delete="deleteArticle" />

		<my-button @click="showDialogAddArticle"
							 style="margin: 15px 0">
			Add article
		</my-button>
	</div>
</template>

<script>
	import ArticleForm from "@/components/Articles/ArticleForm";
	import ArticleList from "@/components/Articles/ArticleList";

	export default {
		name: "articles",
		components: {
			ArticleForm, ArticleList
		},
		data() {
			return {
				users: [],
				articles: [],
				dialogAddArticleVisible: false,
				contentSearch: ""
			}
		},
		methods: {
			async getListUsers() {
				const res = await fetch('https://localhost:44364/api/content/user/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ search: "" })
				});

				const finalRes = await res.json();
				const users = finalRes.users.map(user => ({
					countryId:   user.countryId,
					countryName: user.countryName,
					cityId:      user.cityId,
					cityName:    user.cityName,
					userId:      user.userId,
					userEmail:   user.userEmail
				}));

				this.users = users;
			},
			async getListArticles() {
				const res = await fetch('https://localhost:44364/api/content/article/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ contentName: "" })
				});

				const finalRes = await res.json();
				const articles = finalRes.articles.map(article => ({
					id:          article.id,
					authorId:    (article.author !== null) ? (article.author.userId) : '-1',
					authorEmail: (article.author !== null) ? (article.author.userEmail) : 'Deleted user',
					name:        article.name,
					category:    article.category,
					text:        article.text
				}));
					
				this.articles = articles.filter(article => article.authorId != '-1');
			},
			async addArticle(article) {
				this.dialogAddArticleVisible = false;
				const res = await fetch('https://localhost:44364/api/content/article/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						userId:          article.authorId,
						contentName:     article.name,
						contentCategory: article.category,
						articleText:     article.text
					})
				})
					.then(response => response.json())

				article.id = res.id;
				this.articles.push(article);
			},
			async deleteArticle(article) {
				this.articles = this.articles.filter(a => a.id !== article.id);
				await fetch('https://localhost:44364/api/content/article/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: article.id })
				})
					.then(response => response.json())
			},
			showDialogAddArticle() {
				this.getListUsers();
				this.dialogAddArticleVisible = true;
			},
		},
		mounted() {
			this.getListUsers();
			this.getListArticles();
		},
		computed: {
			filteredArticles() {
				return this.articles
					.filter(article => article.name.toLowerCase().indexOf(this.contentSearch.toLowerCase()) > -1)
			},
		},
	}
</script>

<style>
</style>