<template>
  <div>
    <v-card class="mb-2">
        <v-card-text>     
          <v-simple-table>
            <template v-slot:default>
              <tbody>
                <tr>
                  <td width="350px">
                    <strong>Focusconcept {{attributeKey+1}} <!-- [{{groupKey}}/{{attributeKey}}] --></strong><br>
                    {{ templateData.attribute }} | {{attribute.display}} |
                  </td>
                  <td>
                    {{templateData.template.focus[0].conceptId}} | {{attributeValue.display}} |
                  </td>
                  <td>
                    <v-btn small target="_blank" :href="'https://terminologie.nictiz.nl/art-decor/snomed-ct?conceptId='+templateData.template.focus[0].conceptId">
                      <v-icon>link</v-icon>
                    </v-btn>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
        </v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  name: 'TemplateAttribute',
  data: () => {
    return {
      select: null,
      attribute: 'laden...',
      attributeValue: 'laden...',
      items: [],
      search: null,
      loading: {
        'attribute': false,
        'attributeValue' : false,
      },
    }
  },
  props: ['templateData', 'attributeKey', 'groupKey'],
  methods: {
    retrieveAttributeTerms (conceptid) {
      var that = this
      return new Promise((resolve, reject) => {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attribute = {
            'display' : response.data.fsn.term,
            'preferred' : response.data.pt.term,
          }
          that.loading.attribute = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        }).catch((error) => {
          that.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateNestedFocusPrecoordinated] ['+error+']')
          
          reject('Error retrieveAttribute')
        })
      })
    },
    retrieveAttributeValueTerms (conceptid) {
      var that = this
      return new Promise(function(resolve) {
        var branchVersion = encodeURI(that.requestedTemplate.snomedBranch + '/' + that.requestedTemplate.snomedVersion)
        that.$snowstorm.get('https://snowstorm.test-nictiz.nl/'+ branchVersion +'/concepts/'+conceptid)
        .then((response) => {
          that.attributeValue = {
            'display': response.data.fsn.term,
            'preferred' : response.data.pt.term,
          }
          that.loading.attributeValue = false
          resolve({
            'display': response.data.fsn.term,
            'preferred': response.data.pt.term,
          })
        })
        .catch(() => {
          setTimeout(() => {
            that.retrieveAttributeValueTerms (conceptid)
          }, 5000)
          that.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
        })
      })
    },
    setItems(response) {
      var output = []
      var i;
      for (i=0; i < response.length; i++){
        output.push({
          'id' : response[i]['conceptId'],
          'searchString' : response[i]['fsn']['term'] + ' ' + response[i]['pt']['term'],
          'display' : response[i]['fsn']['term'],
          'preferred' : response[i]['pt']['term'],
        })
      }
      this.items = output
      this.loading = false
      return true;
    }
  },
  watch: {
  },
  computed: {
    requestedTemplate(){
        return this.$store.state.templates.requestedTemplate
    },
    thisComponent(){
      return this.componentData
    }
  },
  mounted: function(){
    this.$store.dispatch('templates/saveAttribute', 
      {
        'groupKey':this.groupKey, 
        'attributeKey': this.attributeKey, 
        'attribute' : {
          'id':'....', 
          'display':'....',
          }, 
        'concept': {
          'id' : '.....',
          'display' : '....',
        },
      })
    this.retrieveAttributeTerms(this.templateData.attribute).then((attribute)=>(
      this.retrieveAttributeValueTerms(this.templateData.template.focus[0].conceptId)).then((attributeValue)=>
        this.$store.dispatch('templates/saveAttribute', {
          'groupKey':this.groupKey, 'attributeKey': this.attributeKey, 'attribute' : {
              'id':this.templateData.attribute,
              'display': attribute.preferred,
              'preferred': attribute.preferred,
            }, 'concept': {
              'id': this.templateData.template.focus[0].conceptId,
              'display': attributeValue.preferred,
              'preferred': attributeValue.preferred,
            }
        })
    ).catch(()=>{
      this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
      this.retrieveAttributeValueTerms(this.templateData.template.focus[0].conceptId)
    })).catch(()=>{
      this.$store.dispatch('templates/addErrormessage', 'Er is een fout opgetreden bij het ophalen van een term. [templateAttributePrecoordinated]')
      this.retrieveAttributeTerms(this.templateData.attribute)
    })
    this.retrieved = true
  }
}
</script>

<style scoped>
</style>