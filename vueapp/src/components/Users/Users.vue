<template>
	<div>
		<my-input v-model="countrySearch"
							type="text"
							placeholder="Country search"></my-input>

		<my-input v-model="citySearch"
							type="text"
							placeholder="City search"></my-input>

		<my-input v-model="userSearch"
							type="text"
							placeholder="User search"></my-input>

		<my-dialog v-model:show="dialogAddUserVisible">
			<user-form :cities="cities"
								 @add="addUser" />
		</my-dialog>
		<user-list :users="filteredUsers"
							 @delete="deleteUser" />

		<my-button @click="showDialogAddUser"
							 style="margin: 15px 0">
			Add user
		</my-button>
	</div>
</template>

<script>
  import UserForm from "@/components/Users/UserForm";
	import UserList from "@/components/Users/UserList";

	export default {
		name: "users",
    components: {
      UserForm, UserList
		},
    data() {
			return {
				cities: [],
				users: [],
				dialogAddUserVisible: false,
				countrySearch: '',
				citySearch: '',
				userSearch: ''
      }
		},
		methods: {
			async getListCities() {
				const res = await fetch('https://localhost:44364/api/content/city/getList', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ search: "" })
				});

				const finalRes = await res.json();
				const cities = finalRes.cities.map(city => ({
					countryId: city.countryId,
					countryName: city.countryName,
					cityId: city.cityId,
					cityName: city.cityName
				}));

				this.cities = cities;
			},
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
			async addUser(user) {
				this.dialogAddUserVisible = false;
				const res = await fetch('https://localhost:44364/api/content/user/add', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({
						countryId:   user.countryId,
						countryName: user.countryName,
						cityId:      user.cityId,
						cityName:    user.cityName,
						userEmail:   user.userEmail
					})
				})
					.then(response => response.json())

				user.userId = res.id;
				this.users.push(user);
			},
			async deleteUser(user) {
				this.users = this.users.filter(u => u.userId !== user.userId);
				await fetch('https://localhost:44364/api/content/user/delete', {
					method: 'POST',
					headers: {
						'Content-Type': 'application/json'
					},
					body: JSON.stringify({ Id: user.userId })
				})
					.then(response => response.json())
			},
			showDialogAddUser() {
				this.getListCities();
				this.getListUsers();
				this.dialogAddUserVisible = true;
			},
		},
		mounted() {
			this.getListCities();
			this.getListUsers();
		},
		computed: {
			filteredUsers() {
				return this.users
					.filter(user => user.countryName.toLowerCase().indexOf(this.countrySearch.toLowerCase()) > -1)
					.filter(user => user.cityName.toLowerCase().indexOf(this.citySearch.toLowerCase()) > -1)
					.filter(user => user.userEmail.toLowerCase().indexOf(this.userSearch.toLowerCase()) > -1)	
			},
		},
  }
</script>

<style>
</style>