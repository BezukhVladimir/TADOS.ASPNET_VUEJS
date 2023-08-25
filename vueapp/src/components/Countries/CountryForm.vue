<template>
	<div>
		<form @submit.prevent>
			<h3>Add country</h3>
			<my-input v-model="country.name"
								type="text"
								placeholder="Name"></my-input>
			<my-button style="align-self: flex-end; margin-top: 15px"
								 @click="addCountry">
				Add!
			</my-button>
		</form>
	</div>
</template>

<script>
	export default {
		data() {
			return {
				country: {
					id: '',
					name: ''
				}
			}
		},
		methods: {
			addCountry() {
				this.$emit('add', this.country)
				this.country = {
					id: '',
					name: ''
				}
			}
		}
	}
</script>

<style>
	form {
		display: flex;
		flex-direction: column;
	}
</style>