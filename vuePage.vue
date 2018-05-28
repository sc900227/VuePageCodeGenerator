<style lang="less">
</style>

<template>
    <div>
      <Card>
        <p slot="title">
          <Icon type="help-buoy"></Icon>$Title$
        </p>
            <crud-table :table-data="tableData" :columns="columns" :total="total" :page-option="pageOption"
                          :on-ok="handleOk" 
                          :table-loading="tableLoading"
                          @on-pre-add="handlePreAdd"
                          @on-pre-edit="handlePreEdit"
                          @on-page-change="handlePageChange"
                          @on-sort-change="handleSort"
                          @on-delete="handleDelete">
                  <Form slot="modal-content" ref="crudItem"
                        :model="crudItem" 
                        :rules="ruleValidate" label-position="right" :label-width="100">
                        $FormItem$
                  </Form>
                  <Row slot="filter" :gutter="16">
                    <Col span="8">
                        <Input v-model="filterEnter.textInput" placeholder="试剂盒名称..."></Input>
                    </Col>
                    
                    <Button type="primary" icon="search" @click="fetchData">搜索</Button>
                    
                </Row>
              </crud-table>
      </Card>
    </div>
</template>

<script>
import CrudTable from "../common/crudTable.vue";

export default {
  name: "reagentinfo-page",
  components: {
    CrudTable
  },
  data() {
    return {
      columns: $Columns$,
      tableData: [],
      pageOption: { pageIndex: 1, pageSize: 10 },
      total: 0,
      tableLoading: true,
      filterEnter: {},
      orderBy: "ID",
      url: "api/services/app/ReagentInfo/",
      header: {
        headers: { "Content-Type": "application/json" }
      },
      crudItem: {
        $CrudItem$
      },
      ruleValidate: {
        $RuleValidate$
      }
    };
  },
  mounted() {
    this.fetchData();
  },
  computed: {
    filterKey() {
      let filter = this.filterEnter;
      let strFilter = [];
      let str = {};
      if (filter) {
        if (filter.textInput) {
          str.ColumName = "ReagentName";
          str.ColumValue = filter.textInput;
          strFilter.push(str);
          return strFilter;
        }
        return "";
      } else {
        return "";
      }
    }
  },
  methods: {
    fetchData() {
      let vm = this;

      vm.tableLoading = true;
      let params = {
        Sorting: vm.orderBy || undefined,
        SkipCount: (vm.pageOption.pageIndex - 1) * vm.pageOption.pageSize,
        MaxResultCount: vm.pageOption.pageSize,
        Filters: vm.filterKey || undefined
      };

      vm.$http
        .post(vm.url + "$Select$", params, vm.header)
        .then(response => {
          console.log(response);
          vm.tableData = response.data.Result["Items"];
          vm.total = Number(response.data.Result["TotalCount"]);
          vm.tableLoading = false;
        })
        .catch(error => {
          console.log(error);
        });
    },
    handleSort(order) {
      this.orderBy = order.replace(" ", "-");
      this.fetchData();
    },
    handlePageChange(pageOpt) {
      let vm = this;
      vm.pageOption = JSON.parse(JSON.stringify(pageOpt));
      vm.fetchData();
    },
    handleOk() {
      let vm = this;
      let params = {
        $Params$
      };

      return new Promise(resolve => {
        vm.$refs["crudItem"].validate(valid => {
          if (valid) {
            let promise = null;
            vm.crudItem.Id = vm.crudItem.Id || undefined;
            if (!vm.crudItem.Id) {
              promise = vm.$http.post(
                vm.url + "$Insert$",
                params,
                vm.header
              );
            } else {
              promise = vm.$http.post(
                vm.url + "$Update$",
                params,
                vm.header
              );
            }

            promise
              .then(res => {
                resolve(true);
                vm.fetchData();
              })
              .catch(err => {
                console.log(err);
              });
          } else {
            resolve(false);
          }
        });
      });
    },
    handlePreAdd() {
      let item = this.crudItem;
      //清空所有值
      for (var prop in item) {
        item[prop] = null;
      }
    },
    handlePreEdit(val) {
      let item = this.crudItem;
      //赋值所有值
      for (var prop in item) {
        item[prop] = val[prop];
      }
    },
    handleDelete(val) {
      let vm = this;
      vm.$http
        .delete(vm.url + "$Delete$?Id=" + val.Id)
        .then(() => {
          vm.$Message.success("已成功删除");
          vm.fetchData();
        })
        .catch(error => {
          console.log(error);
        });
    }
  }
};
</script>