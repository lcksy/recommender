一、http://www.tuicool.com/articles/FzmQziz

二、简介：
        Mahout使用了Taste来提高协同过滤算法的实现，它是一个基于Java实现的可扩展的，高效的推荐引擎。
    Taste既实现了最基本的基于用户的和基于内容的推荐算法，同时也提供了扩展接口，使用户可以方便的定义和实现自己的推荐算法。
    同时，Taste不仅仅只适用于Java应用程序，它可以作为内部服务器的一个组件以HTTP和Web Service的形式向外界提供推荐的逻辑。
    Taste的设计使它能满足企业对推荐引擎在性能、灵活性和可扩展性等方面的要求

三、相关接口介绍
    3.1. DataModel: 是用户喜好信息的抽象接口，它的具体实现支持从任意类型的数据源抽取用户喜好信息。
                    Taste 默认提供 JDBCDataModel 和 FileDataModel，分别支持从数据库和文件中读取用户的喜好信息。

    3.2. UserSimilarity 和 ItemSimilarity: UserSimilarity 用于定义两个用户间的相似度，它是基于协同过滤的推荐引擎的核心部分，可以用来计算用户的“邻居”，
                                           这里我们将与当前用户口味相似的用户称为他的邻居。ItemSimilarity 类似的，计算内容之间的相似度。

    3.3. UserNeighborhood: 用于基于用户相似度的推荐方法中，推荐的内容是基于找到与当前用户喜好相似的邻居用户的方式产生的。
                           UserNeighborhood 定义了确定邻居用户的方法，具体实现一般是基于 UserSimilarity 计算得到的。

    3.4. Recommender: 是推荐引擎的抽象接口，Taste 中的核心组件。程序中，为它提供一个 DataModel，它可以计算出对不同用户的推荐内容。实际应用中，
                      主要使用它的实现类 GenericUserBasedRecommender 或者 GenericItemBasedRecommender，分别实现基于用户相似度的推荐引擎或者基于内容的推荐引擎。

    3.5. RecommenderEvaluator：评分器。

    3.6. RecommenderIRStatsEvaluator：搜集推荐性能相关的指标，包括准确率、召回率等等。

四、UserSimilarity 和 ItemSimilarity 相似度实现有以下几种
    4.1. CityBlockSimilarity： 基于Manhattan距离相似度
    4.2. EuclideanDistanceSimilarity： 基于欧几里德距离计算相似度
    4.3. LogLikelihoodSimilarity： 基于对数似然比的相似度
    4.4. PearsonCorrelationSimilarity： 基于皮尔逊相关系数计算相似度
    4.5. SpearmanCorrelationSimilarity： 基于皮尔斯曼相关系数相似度
    4.6. TanimotoCoefficientSimilarity： 基于谷本系数计算相似度
    4.7. UncenteredCosineSimilarity： 计算 Cosine 相似度

五、UserNeighborhood 主要实现有两种
    5.1. NearestNUserNeighborhood： 对每个用户取固定数量N个最近邻居
    5.2. ThresholdUserNeighborhood： 对每个用户基于一定的限制，取落在相似度限制以内的所有用户为邻居

六、Recommender分为以下几种实现
    6.1. GenericUserBasedRecommender：基于用户的推荐引擎
    6.2. GenericBooleanPrefUserBasedRecommender：基于用户的无偏好值推荐引擎
    6.3. GenericItemBasedRecommender：基于物品的推荐引擎
    6.4. GenericBooleanPrefItemBasedRecommender：基于物品的无偏好值推荐引擎

七、RecommenderEvaluator有以下几种实现
    7.1. AverageAbsoluteDifferenceRecommenderEvaluator ：计算平均差值
    7.2. RMSRecommenderEvaluator ：计算均方根差

    注：RecommenderIRStatsEvaluator的实现类是GenericRecommenderIRStatsEvaluator

八、例子
    8.1. 基于用户的推荐，以FileDataModel为例
        File modelFile modelFile = new File("intro.csv");
        DataModel model = new FileDataModel(modelFile);

        //用户相似度，使用基于皮尔逊相关系数计算相似度
        UserSimilarity similarity = new PearsonCorrelationSimilarity(model);

        //选择邻居用户，使用NearestNUserNeighborhood实现UserNeighborhood接口，选择邻近的4个用户
        UserNeighborhood neighborhood = new NearestNUserNeighborhood(4, similarity, model);
        
        Recommender recommender = new GenericUserBasedRecommender(model, neighborhood, similarity);

        //给用户1推荐4个物品
        List<RecommendedItem> recommendations = recommender.recommend(1, 4);

        for (RecommendedItem recommendation : recommendations) {
            System.out.println(recommendation);
        }
        
        注意： 1. FileDataModel要求输入文件中的字段分隔符为逗号或者制表符，如果你想使用其他分隔符，你可以扩展一个FileDataModel的实现，例如，mahout中已经提供了一个解析MoiveLens的数据集（分隔符为 :: ）的实现GroupLensDataModel。
              2. 对相同用户重复获得推荐结果，我们可以改用CachingRecommender来包装GenericUserBasedRecommender对象，将推荐结果缓存起来：
                Recommender cachingRecommender = new CachingRecommender(recommender);

        上面代码可以在main方法中直接运行，然后，我们可以获取推荐模型的评分

        //使用平均绝对差值获得评分
        RecommenderEvaluator evaluator = new AverageAbsoluteDifferenceRecommenderEvaluator();

        // 用RecommenderBuilder构建推荐引擎
        RecommenderBuilder recommenderBuilder = new RecommenderBuilder() 
        {
	        @Override
	        public Recommender buildRecommender(DataModel model) throws TasteException 
            {
		        UserSimilarity similarity = new PearsonCorrelationSimilarity(model);
		        UserNeighborhood neighborhood = new NearestNUserNeighborhood(4, similarity, model);
		        return new GenericUserBasedRecommender(model, neighborhood, similarity);
	        }
        };
        // Use 70% of the data to train; test using the other 30%.
        double score = evaluator.evaluate(recommenderBuilder, null, model, 0.7, 1.0);
        System.out.println(score);

        接下来，可以获取推荐结果的查准率和召回率
        RecommenderIRStatsEvaluator statsEvaluator = new GenericRecommenderIRStatsEvaluator();
        // Build the same recommender for testing that we did last time:
        RecommenderBuilder recommenderBuilder = new RecommenderBuilder() 
        {
	        @Override
	        public Recommender buildRecommender(DataModel model) throws TasteException 
            {
		        UserSimilarity similarity = new PearsonCorrelationSimilarity(model);
		        UserNeighborhood neighborhood = new NearestNUserNeighborhood(4, similarity, model);
		        return new GenericUserBasedRecommender(model, neighborhood, similarity);
	        }
        };
        // 计算推荐4个结果时的查准率和召回率
        IRStatistics stats = statsEvaluator.evaluate(recommenderBuilder,null, model, null, 4,
		        GenericRecommenderIRStatsEvaluator.CHOOSE_THRESHOLD,1.0);
        System.out.println(stats.getPrecision());
        System.out.println(stats.getRecall());

    8.2. 基于物品的推荐, 代码大体相似，只是没有了UserNeighborhood，然后将上面代码中的User换成Item即可，完整代码如下
        File modelFile modelFile = new File("intro.csv");
        DataModel model = new FileDataModel(new File(file));
        // Build the same recommender for testing that we did last time:
        RecommenderBuilder recommenderBuilder = new RecommenderBuilder() 
        {
            @Override
            public Recommender buildRecommender(DataModel model) throws TasteException 
            {
	            ItemSimilarity similarity = new PearsonCorrelationSimilarity(model);
	            return new GenericItemBasedRecommender(model, similarity);
            }
        };
        //获取推荐结果
        List<RecommendedItem> recommendations = recommenderBuilder.buildRecommender(model).recommend(1, 4);
        for (RecommendedItem recommendation : recommendations) 
        {
            System.out.println(recommendation);
        }

        //计算评分
        RecommenderEvaluator evaluator = new AverageAbsoluteDifferenceRecommenderEvaluator();
        // Use 70% of the data to train; test using the other 30%.
        double score = evaluator.evaluate(recommenderBuilder, null, model, 0.7, 1.0);
        System.out.println(score);

        //计算查全率和查准率
        RecommenderIRStatsEvaluator statsEvaluator = new GenericRecommenderIRStatsEvaluator();
        // Evaluate precision and recall "at 2":
        IRStatistics stats = statsEvaluator.evaluate(recommenderBuilder,
	        null, model, null, 4,
	        GenericRecommenderIRStatsEvaluator.CHOOSE_THRESHOLD,
	        1.0);
        System.out.println(stats.getPrecision());
        System.out.println(stats.getRecall()); 

