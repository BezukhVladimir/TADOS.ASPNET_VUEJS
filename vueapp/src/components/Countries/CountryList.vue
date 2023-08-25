<template>
  <div v-if="countries.length > 0">
    <h3>List of countries</h3>
    <country-item v-for="country in countries"
                  :country="country"
                  :key="country.id"
                  @delete="$emit('delete', country)" />
  </div>
  <h2 v-else style="color: red">
    List of countries is empty
  </h2>
</template>

<script>
  import CountryItem from "@/components/Countries/CountryItem";

  export default {
    components: {
      CountryItem
    },
    props: {
      countries: {
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