<template>
  <div v-if="cities.length > 0">
    <h3>List of cities</h3>
    <city-item v-for="city in cities"
               :city="city"
               :key="city.cityId"
               @delete="$emit('delete', city)" />
  </div>
  <h2 v-else style="color: red">
    List of cities is empty
  </h2>
</template>

<script>
  import CityItem from "@/components/Cities/CityItem";

  export default {
    components: {
      CityItem
    },
    props: {
      cities: {
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